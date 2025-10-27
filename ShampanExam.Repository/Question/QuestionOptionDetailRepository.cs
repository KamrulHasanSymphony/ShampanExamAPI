using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanTailor.ViewModel.QuestionVM;
using System.Data;
using System.Data.SqlClient;

namespace ShampanTailor.Repository.Question
{
    public class QuestionOptionDetailRepository : CommonRepository
    {
        #region Insert
        public async Task<ResultVM> Insert(QuestionOptionDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                INSERT INTO QuestionOptionquestionSetDetailList
                (
                    QuestionHeaderId, QuestionOption, QuestionAnswer
                )
                VALUES
                (
                    @QuestionHeaderId, @QuestionOption, @QuestionAnswer
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@QuestionHeaderId", vm.QuestionHeaderId);
                    cmd.Parameters.AddWithValue("@QuestionOption", vm.QuestionOption);
                    cmd.Parameters.AddWithValue("@QuestionAnswer", vm.QuestionAnswer);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                result.Status = "Success";
                result.Message = "QuestionOptionDetail inserted successfully.";
                result.Id = vm.Id.ToString();
                result.DataVM = vm;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }
        #endregion

        #region List
        public ResultVM List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                SELECT
                    ISNULL(M.Id,0) AS Id,
                    ISNULL(M.QuestionHeaderId,0) AS QuestionHeaderId,
                    ISNULL(M.QuestionOption,'') AS QuestionOption,
                    ISNULL(M.QuestionAnswer,'') AS QuestionAnswer
                FROM QuestionOptionquestionSetDetailList M
                WHERE 1=1 ";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);
                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "QuestionOptionquestionSetDetailList data retrieved successfully.";
                result.DataVM = dataTable;

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }
        #endregion
    }
}
