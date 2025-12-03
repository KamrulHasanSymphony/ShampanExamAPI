using ShampanExam.Repository.Common;
using ShampanExam.Repository.Exam;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.Exam;
using ShampanExam.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.Service.Exam
{
    public class ExaminerService
    {
        public async Task<ResultVM> QuestionList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            ExaminerRepository _repo = new ExaminerRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            SqlConnection conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
            SqlTransaction transaction = Vtransaction;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                if (transaction == null)
                    transaction = conn.BeginTransaction();

                result = await _repo.QuestionList(conditionalFields, conditionalValues, conn, transaction, vm);

                if (Vtransaction == null && transaction != null)
                {
                    if (result.Status == "Success")
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }
            catch (System.Exception ex)
            {
                if (transaction != null && Vtransaction == null)
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

        public async Task<ResultVM> QuestionAnsList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            ExaminerRepository _repo = new ExaminerRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            SqlConnection conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
            SqlTransaction transaction = Vtransaction;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                if (transaction == null)
                    transaction = conn.BeginTransaction();

                result = await _repo.QuestionAnsList(conditionalFields, conditionalValues, conn, transaction, vm);

                if (Vtransaction == null && transaction != null)
                {
                    if (result.Status == "Success")
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }
            catch (System.Exception ex)
            {
                if (transaction != null && Vtransaction == null)
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

        public async Task<ResultVM> Insert(List<QuestionVM> Answers)
        {
            ExaminerRepository _repo = new ExaminerRepository();
            CommonRepository _commonRepo = new CommonRepository();
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

                //#region Check Exist Data
                //string[] conditionField = { "Code" };
                //string[] conditionValue = { ""};

                //bool exist = _commonRepo.CheckExists("Exams", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exists!";
                //    throw new Exception("Data Already Exists!");
                //}
                //#endregion
                foreach (var item in Answers)
                {
                   result = await _repo.UpdateMarks(item, conn, transaction);


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

        public async Task<ResultVM> ExamSubmit(QuestionVM Answers)
        {
            ExaminerRepository _repo = new ExaminerRepository();
            CommonRepository _commonRepo = new CommonRepository();
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


                
                        result = await _repo.ExamSubmit(Answers, conn, transaction);




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
    }
}
