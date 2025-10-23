using ShampanExam.Repository.Exam;
using ShampanExam.ViewModel.CommonVMs;
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
    public class ExamService
    {
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            SqlConnection conn = VcurrConn ?? new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
            SqlTransaction transaction = Vtransaction;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                if (transaction == null)
                    transaction = conn.BeginTransaction();

                result = await _repo.List(conditionalFields, conditionalValues, conn, transaction, vm);

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
    }
}
