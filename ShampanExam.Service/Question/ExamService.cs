using ShampanExam.Repository.Common;
using ShampanExam.Repository.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ShampanExam.Service.Question
{
    public class ExamService
    {
        CommonRepository _commonRepo = new CommonRepository();

        // Insert Method
        public async Task<ResultVM> Insert(ExamVM exam)
        {
            string CodeGroup = "Exam";
            string CodeName = "Exam";
            ExamRepository _repo = new ExamRepository();
            _commonRepo = new CommonRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            CommonVM commonVM = new CommonVM();

            commonVM.Group = "Exam";
            commonVM.Name = "Exam";

            _commonRepo = new CommonRepository();
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();



                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, exam.Date, exam.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    exam.Code = code;


                    result = await _repo.Insert(exam, conn, transaction);
                    if (result.Status == "Success")
                    {
                        foreach (var item in exam.automatedExamDetailList)
                        {
                            item.Id = Convert.ToInt32(result.Id);
                            var resultt = await _repo.DetailsInsert(item, conn, transaction);
                            if (result.Status != "Success")
                            {
                                throw new Exception(resultt.Message);
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

                    }

                }
                else
                {
                    throw new Exception("Code Generation Failed!");
                }
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                result.Code = exam.Code;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }

        // Update Method
        public async Task<ResultVM> Update(ExamVM exam)
        {
            ExamRepository _repo = new ExamRepository();
            _commonRepo = new CommonRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = exam.Id.ToString(), DataVM = exam };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                #region Check Exist Data
                string[] conditionField = { "Id not", "Code" };
                string[] conditionValue = { exam.Id.ToString(), exam.Code.Trim() };

                bool exist = _commonRepo.CheckExists("Exams", conditionField, conditionValue, conn, transaction);
                if (exist)
                {
                    result.Message = "Data Already Exists!";
                    throw new Exception("Data Already Exists!");
                }
                #endregion

                result = await _repo.Update(exam, conn, transaction);

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

        // Delete (Archive) Method
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", IDs = vm.IDs };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                result = await _repo.Delete(vm, conn, transaction);

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

        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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


        public async Task<ResultVM> GetExamInfoReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetExamInfoReport(conditionalFields, conditionalValues, vm, conn, transaction);

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
                result.Message = ex.Message;
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


        // GetProcessedData Method
        public async Task<ResultVM> GetProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

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
        // GetProcessedData Method
        public async Task<ResultVM> ListOfProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM resuletlist = new ResultVM { Status = "Fail", Message = "Error" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                resuletlist = await _repo.ListOfProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && resuletlist.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(resuletlist.Message);

                return resuletlist;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                resuletlist.Message = ex.Message;
                resuletlist.ExMessage = ex.ToString();
                return resuletlist;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }

        //// ListAsDataTable Method
        //public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    ExamRepository _repo = new ExamRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
        //        conn.Open();
        //        isNewConnection = true;
        //        transaction = conn.BeginTransaction();

        //        result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, vm, conn, transaction);

        //        if (isNewConnection && result.Status == "Success")
        //            transaction.Commit();
        //        else
        //            throw new Exception(result.Message);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection) transaction.Rollback();
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null) conn.Close();
        //    }
        //}

        //// Dropdown Method
        //public async Task<ResultVM> Dropdown()
        //{
        //    ExamRepository _repo = new ExamRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
        //        conn.Open();
        //        isNewConnection = true;
        //        transaction = conn.BeginTransaction();

        //        result = await _repo.Dropdown(conn, transaction);

        //        if (isNewConnection && result.Status == "Success")
        //            transaction.Commit();
        //        else
        //            throw new Exception(result.Message);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection) transaction.Rollback();
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null) conn.Close();
        //    }
        //}

        // GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ExamRepository _repo = new ExamRepository();
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


        // GetProcessedData Method
        public async Task<ResultVM> GetRandomProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetRandomProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> GetUserRandomProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetUserRandomProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

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

        // GetGridData Method
        public async Task<ResultVM> GetRandomGridData(GridOptions options)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetRandomGridData(options, conn, transaction);

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
    }
}
