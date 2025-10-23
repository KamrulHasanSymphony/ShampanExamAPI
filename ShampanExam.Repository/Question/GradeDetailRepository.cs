using Newtonsoft.Json;
using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.QuestionVM;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ShampanExam.Repository.Question
{
    public class GradeDetailRepository : CommonRepository
    {
        #region Insert
        public async Task<ResultVM> Insert(GradeDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                INSERT INTO GradeDetails
                (
                    GradeId, Grade, MinPercentage, MaxPercentage, GradePoint, GradePointNote
                )
                VALUES
                (
                    @GradeId, @Grade, @MinPercentage, @MaxPercentage, @GradePoint, @GradePointNote
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@GradeId", vm.GradeId);
                    cmd.Parameters.AddWithValue("@Grade", vm.Grade);
                    cmd.Parameters.AddWithValue("@MinPercentage", vm.MinPercentage);
                    cmd.Parameters.AddWithValue("@MaxPercentage", vm.MaxPercentage);
                    cmd.Parameters.AddWithValue("@GradePoint", vm.GradePoint);
                    cmd.Parameters.AddWithValue("@GradePointNote", vm.GradePointNote);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                result.Status = "Success";
                result.Message = "GradeDetail inserted successfully.";
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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                SELECT 
                    ISNULL(M.Id,0) AS Id, 
                    ISNULL(M.GradeId,0) AS GradeId,
                    ISNULL(M.Grade,'') AS Grade,
                    ISNULL(M.MinPercentage,0) AS MinPercentage,
                    ISNULL(M.MaxPercentage,0) AS MaxPercentage,
                    ISNULL(M.GradePoint,0) AS GradePoint,
                    ISNULL(M.GradePointNote,'') AS GradePointNote
                FROM GradeDetails M
                WHERE 1=1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id=@Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Details Data retrieved successfully.";
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
