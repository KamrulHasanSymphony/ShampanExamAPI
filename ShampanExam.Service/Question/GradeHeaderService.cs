using ShampanExam.Repository.Common;
using ShampanExam.Repository.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using System.Data.SqlClient;

namespace ShampanExam.Service.Grades
{
    public class GradeHeaderService
    {
        CommonRepository _commonRepo = new CommonRepository();

        // Insert
        public async Task<ResultVM> Insert(GradeHeaderVM gradeHeader)
        {
            GradeHeaderRepository _repo = new GradeHeaderRepository();
            GradeDetailRepository gradequestionSetDetailListRepository = new GradeDetailRepository();
            _commonRepo = new CommonRepository();
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

                if (gradeHeader.gradeDetailList == null || !gradeHeader.gradeDetailList.Any())
                {
                    throw new Exception("GradequestionSetDetailList must have at least one detail!");
                }

                #region Check Exist Data
                string[] conditionField = { "Name" };
                string[] conditionValue = { gradeHeader.Name.Trim() };

                bool exist = _commonRepo.CheckExists("Grades", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion

                result = await _repo.Insert(gradeHeader, conn, transaction);

                gradeHeader.Id = Convert.ToInt32(result.Id);

                if (result.Status.ToLower() == "success")
                {
                    foreach (var gradeDetail in gradeHeader.gradeDetailList)
                    {
                        gradeDetail.GradeId = gradeHeader.Id;

                        var resultDetail = await gradequestionSetDetailListRepository.Insert(gradeDetail, conn, transaction);
                        if (resultDetail.Status.ToLower() != "success")
                        {
                            throw new Exception(resultDetail.Message);
                        }
                    }
                }

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

        // Update
        public async Task<ResultVM> Update(GradeHeaderVM gradeHeader)
        {
            GradeHeaderRepository _repo = new GradeHeaderRepository();
            GradeDetailRepository gradequestionSetDetailListRepository = new GradeDetailRepository();
            _commonRepo = new CommonRepository();
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

                if (gradeHeader.gradeDetailList == null || !gradeHeader.gradeDetailList.Any())
                {
                    throw new Exception("GradequestionSetDetailList must have at least one detail!");
                }

                var record = _commonRepo.DetailsDelete("GradequestionSetDetailList", new[] { "GradeId" }, new[] { gradeHeader.Id.ToString() }, conn, transaction);
                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for GradequestionSetDetailList Data.");
                }

                result = await _repo.Update(gradeHeader, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    foreach (var gradeDetail in gradeHeader.gradeDetailList)
                    {
                        gradeDetail.GradeId = gradeHeader.Id;

                        var resultDetail = await gradequestionSetDetailListRepository.Insert(gradeDetail, conn, transaction);
                        if (resultDetail.Status.ToLower() != "success")
                            throw new Exception(resultDetail.Message);
                    }
                }

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

        // List
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            GradeHeaderRepository _repo = new GradeHeaderRepository();
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

        // GetGridData
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            GradeHeaderRepository _repo = new GradeHeaderRepository();
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

                result = await _repo.GetGridData(options, conn, transaction);

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

        // Dropdown
        public async Task<ResultVM> Dropdown(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            GradeHeaderRepository _repo = new GradeHeaderRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.Dropdown(conditionalFields, conditionalValues, vm, conn, transaction);

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
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
