using Newtonsoft.Json;
using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanTailor.ViewModel.QuestionVM;
using System.Data;
using System.Data.SqlClient;

namespace ShampanTailor.Repository.Question
{
    public class QuestionHeaderRepository : CommonRepository
    {
        #region Insert
        public async Task<ResultVM> Insert(QuestionHeaderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                INSERT INTO QuestionHeaders
                (
                    Code,QuestionSubjectId, QuestionChapterId, QuestionCategorieId, QuestionText, QuestionType, QuestionMark
                )
                VALUES
                (
                   @Code, @QuestionSubjectId, @QuestionChapterId, @QuestionCategorieId, @QuestionText, @QuestionType, @QuestionMark
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Parameters.AddWithValue("@QuestionSubjectId", vm.QuestionSubjectId);
                    cmd.Parameters.AddWithValue("@QuestionChapterId", vm.QuestionChapterId);
                    cmd.Parameters.AddWithValue("@QuestionCategorieId", vm.QuestionCategorieId);
                    cmd.Parameters.AddWithValue("@QuestionText", vm.QuestionText);
                    cmd.Parameters.AddWithValue("@QuestionType", vm.QuestionType);
                    cmd.Parameters.AddWithValue("@QuestionMark", vm.QuestionMark);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                result.Status = "Success";
                result.Message = "QuestionHeader inserted successfully.";
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

        #region Update
        public async Task<ResultVM> Update(QuestionHeaderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = vm.Id.ToString(), DataVM = vm };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                UPDATE QuestionHeaders 
                SET 
                    QuestionSubjectId = @QuestionSubjectId,
                    QuestionChapterId = @QuestionChapterId,
                    QuestionCategorieId = @QuestionCategorieId,
                    QuestionText = @QuestionText,
                    QuestionType = @QuestionType,
                    QuestionMark = @QuestionMark
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@QuestionSubjectId", vm.QuestionSubjectId);
                    cmd.Parameters.AddWithValue("@QuestionChapterId", vm.QuestionChapterId);
                    cmd.Parameters.AddWithValue("@QuestionCategorieId", vm.QuestionCategorieId);
                    cmd.Parameters.AddWithValue("@QuestionText", vm.QuestionText);
                    cmd.Parameters.AddWithValue("@QuestionType", vm.QuestionType);
                    cmd.Parameters.AddWithValue("@QuestionMark", vm.QuestionMark);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result.Status = "Success";
                        result.Message = "QuestionHeader updated successfully.";
                    }
                    else throw new Exception("No rows updated.");
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }
        #endregion

        #region Delete
        public async Task<ResultVM> Delete(QuestionHeaderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = "DELETE FROM QuestionHeaders WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result.Status = "Success";
                        result.Message = "QuestionHeader deleted successfully.";
                    }
                    else throw new Exception("No rows deleted.");
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                if (transaction != null) transaction.Rollback();
            }
            return result;
        }
        #endregion

        #region List
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                SELECT
                    ISNULL(M.Id,0) AS Id,
                     ISNULL(M.Code,'') AS Code,
                    ISNULL(M.QuestionSubjectId,0) AS QuestionSubjectId,
                    ISNULL(M.QuestionChapterId,0) AS QuestionChapterId,
                    ISNULL(M.QuestionCategorieId,0) AS QuestionCategorieId,
                    ISNULL(M.QuestionText,'') AS QuestionText,
                    ISNULL(M.QuestionType,'') AS QuestionType,
                    ISNULL(M.QuestionMark,0) AS QuestionMark
                FROM QuestionHeaders M
                WHERE 1=1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<QuestionHeaderVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new QuestionHeaderVM
                    {
                        Id = row.Field<int>("Id"),
                        QuestionSubjectId = row.Field<int>("QuestionSubjectId"),
                        QuestionChapterId = row.Field<int>("QuestionChapterId"),
                        QuestionCategorieId = row.Field<int>("QuestionCategorieId"),
                        QuestionText = row.Field<string>("QuestionText"),
                        QuestionType = row.Field<string>("QuestionType"),
                        QuestionMark = row.Field<int>("QuestionMark")
                    });
                }

                // ✅ Load Question Option questionSetDetailList
                QuestionOptionDetailRepository optionquestionSetDetailListRepository = new QuestionOptionDetailRepository();
                var optionquestionSetDetailListDataList = optionquestionSetDetailListRepository.List(new[] { "M.QuestionHeaderId" }, conditionalValues, vm, conn, transaction);
                if (optionquestionSetDetailListDataList.Status == "Success" && optionquestionSetDetailListDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var optionquestionSetDetailList = JsonConvert.DeserializeObject<List<QuestionOptionDetailVM>>(json);

                    if (model.Any())
                        model.FirstOrDefault().QuestionOptionquestionSetDetailList = optionquestionSetDetailList;
                }

                // ✅ Load Question Short questionSetDetailList
                QuestionShortDetailRepository shortquestionSetDetailListRepository = new QuestionShortDetailRepository();
                var shortquestionSetDetailListDataList = shortquestionSetDetailListRepository.List(new[] { "M.QuestionHeaderId" }, conditionalValues, vm, conn, transaction);
                if (shortquestionSetDetailListDataList.Status == "Success" && shortquestionSetDetailListDataList.DataVM is DataTable dt2)
                {
                    string json = JsonConvert.SerializeObject(dt2);
                    var shortquestionSetDetailList = JsonConvert.DeserializeObject<List<QuestionShortDetailVM>>(json);

                    if (model.Any())
                        model.FirstOrDefault().QuestionShortquestionSetDetailList = shortquestionSetDetailList;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = model;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }
        #endregion

        #region GetGridData
        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                var data = new GridEntity<QuestionHeaderVM>();

                string sqlQuery = @"
                -- Count query
                SELECT COUNT(DISTINCT H.Id) AS totalcount
                FROM QuestionHeaders H
                WHERE 1 = 1
                " + (options.filter.Filters.Count > 0
                        ? " AND (" + GridQueryBuilder<QuestionHeaderVM>.FilterCondition(options.filter) + ")"
                        : "") + @"

                -- Data query
                SELECT *
                FROM (
                    SELECT ROW_NUMBER() OVER(ORDER BY " +
                        (options.sort.Count > 0
                            ? "H." + options.sort[0].field + " " + options.sort[0].dir
                            : "H.Id DESC") + @") AS rowindex,
                           ISNULL(H.Id,0) AS Id,                      
                           ISNULL(H.Code,'') AS Code,

                           ISNULL(H.QuestionSubjectId,0) AS QuestionSubjectId,
                           ISNULL(H.QuestionChapterId,0) AS QuestionChapterId,
                           ISNULL(H.QuestionCategorieId,0) AS QuestionCategorieId,
                           ISNULL(H.QuestionText,'') AS QuestionText,
                           ISNULL(H.QuestionType,'') AS QuestionType,
                           ISNULL(H.QuestionMark,0) AS QuestionMark
                    FROM QuestionHeaders H
                    WHERE 1 = 1
                                " + (options.filter.Filters.Count > 0
                                                ? " AND (" + GridQueryBuilder<QuestionHeaderVM>.FilterCondition(options.filter) + ")"
                                                : "") + @"
                            ) AS a
                            WHERE rowindex > @skip AND (@take=0 OR rowindex <= @take)";

                data = KendoGrid<QuestionHeaderVM>.GetGridDataQuestions_CMD(options, sqlQuery, "H.Id");

                result.Status = "Success";
                result.Message = "Grid data retrieved successfully.";
                result.DataVM = data;
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


        // Dropdown Method
        public async Task<ResultVM> Dropdown(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    throw new Exception("Database connection fail!");
                }

                string query = @"
                SELECT Id, QuestionText,QuestionType,QuestionMark
                FROM QuestionHeaders
                WHERE 1 = 1
                ORDER BY Id";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new QuestionHeaderVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    QuestionText = row["QuestionText"]?.ToString(),
                    QuestionType = row["QuestionType"]?.ToString(),
                    QuestionMark = Convert.ToInt32(row["QuestionMark"]),
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}
