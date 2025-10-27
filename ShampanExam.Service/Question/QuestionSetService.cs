using ShampanExam.Repository.Common;
using ShampanExam.Repository.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace ShampanExam.Service.Question
{
    public class QuestionSetService
    {
        CommonRepository _commonRepo = new CommonRepository();

        // =========================== INSERT HEADER + questionSetDetailList ===========================
        public async Task<ResultVM> Insert(QuestionSetHeaderVM vm, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                #region Connection and Transaction
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                transaction = Vtransaction ?? conn.BeginTransaction();
                #endregion

                // 🔹 Insert Header
                result = await _repo.Insert(vm, conn, transaction);
                if (result.Status.ToLower() != "success")
                    throw new Exception(result.Message);

                vm.Id = Convert.ToInt32(result.Id);

                // 🔹 Insert questionSetDetailList
                if (vm.questionSetDetailList != null && vm.questionSetDetailList.Any())
                {
                    foreach (var detail in vm.questionSetDetailList)
                    {
                        detail.QuestionSetHeaderId = vm.Id;
                        var detailResult = await _repo.InsertDetail(detail, conn, transaction);
                        if (detailResult.Status.ToLower() != "success")
                            throw new Exception(detailResult.Message);
                    }
                }

                #region Commit
                if (Vtransaction == null)
                    transaction.Commit();
                #endregion

                result.Status = "Success";
                result.Message = "Question Set inserted successfully.";
                result.Id = vm.Id.ToString();
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                    transaction.Rollback();

                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            finally
            {
                if (VcurrConn == null && conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }

            return result;
        }

        // =========================== UPDATE HEADER + questionSetDetailList ===========================
        public async Task<ResultVM> Update(QuestionSetHeaderVM vm, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = vm.Id.ToString() };
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                #region Connection
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                transaction = Vtransaction ?? conn.BeginTransaction();
                #endregion

                // Delete existing questionSetDetailList first
                var delRes = _commonRepo.questionSetDetailListDelete("QuestionSetDetails", new[] { "QuestionSetHeaderId" }, new[] { vm.Id.ToString() }, conn, transaction);
                if (delRes.Status == "Fail")
                    throw new Exception("Error deleting old questionSetDetailList.");

                // Update header
                result = await _repo.Update(vm, conn, transaction);
                if (result.Status.ToLower() != "success")
                    throw new Exception(result.Message);

                // Re-insert questionSetDetailList
                if (vm.questionSetDetailList != null && vm.questionSetDetailList.Any())
                {
                    foreach (var d in vm.questionSetDetailList)
                    {
                        d.QuestionSetHeaderId = vm.Id;
                        var insRes = await _repo.InsertDetail(d, conn, transaction);
                        if (insRes.Status.ToLower() != "success")
                            throw new Exception(insRes.Message);
                    }
                }

                #region Commit
                if (Vtransaction == null)
                    transaction.Commit();
                #endregion

                result.Status = "Success";
                result.Message = "Question Set updated successfully.";
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                    transaction.Rollback();

                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            finally
            {
                if (VcurrConn == null && conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return result;
        }

        // =========================== DELETE MULTIPLE HEADERS ===========================
        public async Task<ResultVM> MultipleDelete(CommonVM vm, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                transaction = Vtransaction ?? conn.BeginTransaction();

                result = await _repo.MultipleDelete(vm, conn, transaction);

                if (Vtransaction == null)
                    transaction.Commit();
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                    transaction.Rollback();
                result.Message = ex.Message;
            }
            finally
            {
                if (VcurrConn == null && conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return result;
        }

        // =========================== LIST ===========================
        // List
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            QuestionSetHeaderRepository _repo = new QuestionSetHeaderRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                result = await _repo.List(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }

        // =========================== GRID DATA ===========================
        public async Task<ResultVM> GetGridData(GridOptions options, string[] fields, string[] values)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            SqlConnection conn = new SqlConnection(DatabaseHelper.GetConnectionString());
            SqlTransaction transaction = null;

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                result = await _repo.GetGridData(options, fields, values, conn, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                result.Message = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return result;
        }

        // =========================== MULTIPLE POST ===========================
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            SqlConnection conn = new SqlConnection(DatabaseHelper.GetConnectionString());
            SqlTransaction transaction = null;

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                result = await _repo.MultiplePost(vm, conn, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                result.Message = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return result;
        }

        // =========================== REPORT PREVIEW ===========================
        public async Task<ResultVM> ReportPreview(string[] fields, string[] values)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            SqlConnection conn = new SqlConnection(DatabaseHelper.GetConnectionString());
            SqlTransaction transaction = null;

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                result = await _repo.ReportPreview(fields, values, conn, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                result.Message = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return result;
        }

        // =========================== DETAIL GRID BY HEADER ===========================
        public async Task<ResultVM> GetQuestionSetDetailDataById(GridOptions options, int headerId)
        {
            QuestionSetRepository _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            SqlConnection conn = new SqlConnection(DatabaseHelper.GetConnectionString());
            SqlTransaction transaction = null;

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                result = await _repo.GetQuestionSetDetailDataById(options, headerId, conn, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                result.Message = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return result;
        }

        public async Task<ResultVM> InsertQuestionSetQuestion(QuestionSetQuestionVM vm, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            var _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };
            SqlConnection conn = null; SqlTransaction transaction = null;

            try
            {
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open) conn.Open();
                transaction = Vtransaction ?? conn.BeginTransaction("");

                result = await _repo.InsertQuestionSetQuestion(vm, conn, transaction);

                if (Vtransaction == null)
                {
                    if (result.Status == "Success") transaction.Commit();
                    else throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) transaction.Rollback();
                result.Message = ex.Message; result.ExMessage = ex.ToString();
            }
            finally
            {
                if (VcurrConn == null && conn?.State == ConnectionState.Open) conn.Close();
            }
            return result;
        }

        public async Task<ResultVM> QuestionSetQuestionList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            var _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };
            SqlConnection conn = null; SqlTransaction transaction = null;

            try
            {
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open) conn.Open();
                transaction = Vtransaction ?? conn.BeginTransaction("");

                result = await _repo.QuestionSetQuestionList(conditionalFields, conditionalValues, vm, conn, transaction);

                if (Vtransaction == null)
                {
                    if (result.Status == "Success") transaction.Commit();
                    else throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) transaction.Rollback();
                result.Message = ex.Message; result.ExMessage = ex.ToString();
            }
            finally
            {
                if (VcurrConn == null && conn?.State == ConnectionState.Open) conn.Close();
            }
            return result;
        }


        public async Task<ResultVM> GetQuestionSetGridDataByQuestion(GridOptions options, string? questionHeaderId)
        {
            var _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                var fields = new List<string>();
                var values = new List<string>();

                if (!string.IsNullOrWhiteSpace(questionHeaderId))
                {
                    // same style as sample: target the D.* alias used inside repo SQL
                    fields.Add("D.QuestionHeaderId");
                    values.Add(questionHeaderId);
                }

                result = await _repo.GetQuestionSetGridDataByQuestion(options, fields.ToArray(), values.ToArray(), conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }
        public async Task<ResultVM> Dropdown(SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            var _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };

            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                // open conn/txn
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open) conn.Open();
                transaction = Vtransaction ?? conn.BeginTransaction("");

                // call repo
                result = await _repo.Dropdown(conn, transaction);

                // commit
                if (Vtransaction == null && transaction != null)
                {
                    if (result.Status == "Success") transaction.Commit();
                    else throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) transaction.Rollback();
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            finally
            {
                if (VcurrConn == null && conn != null && conn.State == ConnectionState.Open) conn.Close();
            }
            return result;
        }

        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            var _repo = new QuestionSetRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };

            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                // open conn/txn
                conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionString());
                if (conn.State != ConnectionState.Open) conn.Open();
                transaction = Vtransaction ?? conn.BeginTransaction("");

                // call repo
                result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, conn, transaction);

                // commit
                if (Vtransaction == null && transaction != null)
                {
                    if (result.Status == "Success") transaction.Commit();
                    else throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) transaction.Rollback();
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            finally
            {
                if (VcurrConn == null && conn != null && conn.State == ConnectionState.Open) conn.Close();
            }
            return result;
        }


    }
}
