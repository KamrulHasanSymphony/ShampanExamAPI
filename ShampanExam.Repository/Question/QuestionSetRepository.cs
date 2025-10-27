using Newtonsoft.Json;
using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ShampanExam.Repository.Question
{
    public class QuestionSetRepository : CommonRepository
    {
        // =========================== INSERT HEADER ===========================
        public async Task<ResultVM> Insert(QuestionSetHeaderVM vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = "0" };

            try
            {
                string query = @"
                INSERT INTO QuestionSetHeaders
                (Name, TotalMark, Remarks, IsActive, IsArchive, CreatedBy, CreatedAt, CreatedFrom)
                VALUES
                (@Name, @TotalMark, @Remarks, @IsActive, @IsArchive, @CreatedBy, GETDATE(), @CreatedFrom);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalMark", vm.TotalMark);
                    cmd.Parameters.AddWithValue("@Remarks", vm.Remarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

                    var id = await cmd.ExecuteScalarAsync();
                    vm.Id = Convert.ToInt32(id);
                    result.Status = "Success";
                    result.Message = "Header inserted successfully.";
                    result.Id = vm.Id.ToString();
                    result.DataVM = vm;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }

            return result;
        }

        // =========================== UPDATE HEADER ===========================
        public async Task<ResultVM> Update(QuestionSetHeaderVM vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error", Id = vm.Id.ToString() };

            try
            {
                string query = @"
                UPDATE QuestionSetHeaders SET
                    Name = @Name,
                    TotalMark = @TotalMark,
                    Remarks = @Remarks,
                    IsActive = @IsActive,
                    LastUpdateBy = @LastUpdateBy,
                    LastUpdateAt = GETDATE(),
                    LastUpdateFrom = @LastUpdateFrom
                WHERE Id = @Id";

                using (SqlCommand cmd = new(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalMark", vm.TotalMark);
                    cmd.Parameters.AddWithValue("@Remarks", vm.Remarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    result.Status = rows > 0 ? "Success" : "Fail";
                    result.Message = rows > 0 ? "Header updated successfully." : "No data updated.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }

            return result;
        }

        // =========================== DELETE HEADER ===========================
        public async Task<ResultVM> MultipleDelete(CommonVM vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };

            try
            {
                string inClause = string.Join(", ", vm.IDs.Select((id, i) => $"@Id{i}"));
                string query = $@"
                UPDATE QuestionSetHeaders
                SET IsArchive = 1, IsActive = 0,
                    LastUpdateBy = @ModifyBy,
                    LastUpdateAt = GETDATE(),
                    LastUpdateFrom = @ModifyFrom
                WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    cmd.Parameters.AddWithValue("@ModifyBy", vm.ModifyBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifyFrom", vm.ModifyFrom ?? (object)DBNull.Value);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    result.Status = rows > 0 ? "Success" : "Fail";
                    result.Message = rows > 0 ? "Header deleted successfully." : "No data deleted.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }

            return result;
        }

        // =========================== LIST HEADER ===========================

        #region List
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dt = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                SELECT 
                    ISNULL(M.Id,0) AS Id, 
                    ISNULL(M.Name,'') AS Name,
                    ISNULL(M.TotalMark,0) AS TotalMark,
                    ISNULL(M.Remarks,'') AS Remarks,
                    ISNULL(M.IsActive,0) AS IsActive,
                    ISNULL(M.IsArchive,0) AS IsArchive,
                    ISNULL(M.CreatedBy,'') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedAt,'yyyy-MM-dd HH:mm'),'') AS CreatedAt,
                    ISNULL(M.LastUpdateBy,'') AS LastUpdateBy,
                    ISNULL(FORMAT(M.LastUpdateAt,'yyyy-MM-dd HH:mm'),'') AS LastUpdateAt
                FROM QuestionSetHeaders M
                WHERE 1=1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                    query += " AND M.Id=@Id ";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
                adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                    adapter.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);

                adapter.Fill(dt);

                var list = dt.AsEnumerable().Select(row => new QuestionSetHeaderVM
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),
                    TotalMark = row.Field<int>("TotalMark"),
                    Remarks = row.Field<string>("Remarks"),
                    IsActive = row.Field<bool>("IsActive"),
                    IsArchive = row.Field<bool>("IsArchive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedAt = row.Field<string>("CreatedAt"),
                    LastUpdateBy = row.Field<string>("LastUpdateBy"),
                    LastUpdateAt = row.Field<string>("LastUpdateAt")
                }).ToList();

                QuestionSetDetailRepository questionsetquestionSetDetailListrepository = new QuestionSetDetailRepository();
                // ✅ Load Design Category questionSetDetailList
                var QuestionquestionSetDetailListDataList = questionsetquestionSetDetailListrepository.List(new[] { "M.QuestionSetHeaderId" }, conditionalValues, vm, conn, transaction);
                if (QuestionquestionSetDetailListDataList.Status == "Success" && QuestionquestionSetDetailListDataList.DataVM is DataTable dt2)
                {
                    string json = JsonConvert.SerializeObject(dt2);
                    var QuestionquestionSetDetailList = JsonConvert.DeserializeObject<List<QuestionSetDetailVM>>(json);

                    if (list.Any())
                        list.FirstOrDefault().questionSetDetailList = QuestionquestionSetDetailList;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = list;

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

        //public async Task<ResultVM> List(string[] fields, string[] values, SqlConnection conn, SqlTransaction tran)
        //{
        //    ResultVM result = new() { Status = "Fail", Message = "Error" };
        //    try
        //    {
        //        string query = @"
        //        SELECT 
        //            ISNULL(M.Id,0) Id,
        //            ISNULL(M.Name,'') Name,
        //            ISNULL(M.TotalMark,0) TotalMark,
        //            ISNULL(M.Remarks,'') Remarks,
        //            ISNULL(M.IsActive,0) IsActive,
        //            ISNULL(M.IsArchive,0) IsArchive,
        //            ISNULL(M.CreatedBy,'') CreatedBy,
        //            FORMAT(M.CreatedAt,'yyyy-MM-dd HH:mm') CreatedAt,
        //            ISNULL(M.CreatedFrom,'') CreatedFrom,
        //            ISNULL(M.LastUpdateBy,'') LastUpdateBy,
        //            FORMAT(M.LastUpdateAt,'yyyy-MM-dd HH:mm') LastUpdateAt,
        //            ISNULL(M.LastUpdateFrom,'') LastUpdateFrom
        //        FROM QuestionSetHeaders M
        //        WHERE 1=1";

        //        query = ApplyConditions(query, fields, values, false);

        //        DataTable dt = new();
        //        SqlDataAdapter adp = CreateAdapter(query, conn, tran);
        //        adp.SelectCommand = ApplyParameters(adp.SelectCommand, fields, values);
        //        adp.Fill(dt);

        //        result.Status = "Success";
        //        result.Message = "Header list retrieved.";
        //        result.DataVM = dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}

        // =========================== LIST AS DATATABLE ===========================
        public async Task<ResultVM> ListAsDataTable(string[] fields, string[] values, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                string query = "SELECT * FROM QuestionSetHeaders WHERE 1=1";
                query = ApplyConditions(query, fields, values, false);

                DataTable dt = new();
                SqlDataAdapter adp = CreateAdapter(query, conn, tran);
                adp.SelectCommand = ApplyParameters(adp.SelectCommand, fields, values);
                adp.Fill(dt);

                result.Status = "Success";
                result.Message = "DataTable list retrieved.";
                result.DataVM = dt;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== DROPDOWN ===========================
        public async Task<ResultVM> Dropdown(SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                string query = @"SELECT Id, Name FROM QuestionSetHeaders WHERE IsActive = 1 ORDER BY Name";

                DataTable dt = new();
                SqlDataAdapter adp = new(query, conn);
                if (tran != null) adp.SelectCommand.Transaction = tran;
                adp.Fill(dt);

                result.Status = "Success";
                result.Message = "Dropdown data retrieved.";
                result.DataVM = dt;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== INSERT DETAIL ===========================
        public async Task<ResultVM> InsertDetail(QuestionSetDetailVM vm, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                string query = @"
                INSERT INTO QuestionSetDetails
                (QuestionSetHeaderId, QuestionHeaderId, QuestionMark)
                VALUES
                (@QuestionSetHeaderId, @QuestionHeaderId, @QuestionMark);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new(query, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@QuestionSetHeaderId", vm.QuestionSetHeaderId);
                    cmd.Parameters.AddWithValue("@QuestionHeaderId", vm.QuestionHeaderId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuestionMark", vm.QuestionMark);
                    var id = await cmd.ExecuteScalarAsync();
                    vm.Id = Convert.ToInt32(id);

                    result.Status = "Success";
                    result.Message = "Detail inserted successfully.";
                    result.Id = vm.Id.ToString();
                    result.DataVM = vm;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }

        // =========================== DETAIL LIST ===========================
        public async Task<ResultVM> DetailList(string[] fields, string[] values, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                string query = @"
                SELECT 
                    ISNULL(D.Id,0) Id,
                    ISNULL(D.QuestionSetHeaderId,0) QuestionSetHeaderId,
                    ISNULL(D.QuestionHeaderId,0) QuestionHeaderId,
                    ISNULL(D.QuestionMark,0) QuestionMark,
                    ISNULL(Q.QuestionText,'') QuestionText,
                    ISNULL(Q.QuestionType,'') QuestionType
                FROM QuestionSetDetails D
                LEFT JOIN QuestionHeaders Q ON D.QuestionHeaderId = Q.Id
                WHERE 1=1";

                query = ApplyConditions(query, fields, values, false);

                DataTable dt = new();
                SqlDataAdapter adp = CreateAdapter(query, conn, tran);
                adp.SelectCommand = ApplyParameters(adp.SelectCommand, fields, values);
                adp.Fill(dt);

                result.Status = "Success";
                result.Message = "Detail list retrieved.";
                result.DataVM = dt;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== GRID DATA ===========================
        public async Task<ResultVM> GetGridData(GridOptions options, string[] fields, string[] values, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                var data = new GridEntity<QuestionSetHeaderVM>();

                string sql = @"
                -- Count query
                SELECT COUNT(*) totalcount FROM QuestionSetHeaders H WHERE 1=1
                " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<QuestionSetHeaderVM>.FilterCondition(options.filter) + ")" : "");

                sql = ApplyConditions(sql, fields, values, false);

                sql += @"
                -- Data query
                SELECT * FROM (
                    SELECT 
                    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
                    ISNULL(H.Id,0) Id,
                    ISNULL(H.Name,'') Name,
                    ISNULL(H.TotalMark,0) TotalMark,
                    ISNULL(H.Remarks,'') Remarks,
                    CASE WHEN H.IsActive=1 THEN 'Active' ELSE 'Inactive' END AS Status,
                    FORMAT(H.CreatedAt,'yyyy-MM-dd HH:mm') CreatedAt
                    FROM QuestionSetHeaders H
                    WHERE 1=1
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<QuestionSetHeaderVM>.FilterCondition(options.filter) + ")" : "") + @"
                ) A
                WHERE rowindex > @skip AND (@take=0 OR rowindex<=@take)";

                data = KendoGrid<QuestionSetHeaderVM>.GetTransactionalQuestionGridData_CMD(options, sql, "H.Id", fields, values);

                result.Status = "Success";
                result.Message = "Grid data retrieved.";
                result.DataVM = data;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== REPORT PREVIEW ===========================
        public async Task<ResultVM> ReportPreview(string[] fields, string[] values, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                string query = @"
                SELECT 
                    ISNULL(H.Id,0) Id,
                    ISNULL(H.Name,'') Name,
                    ISNULL(H.TotalMark,0) TotalMark,
                    ISNULL(H.Remarks,'') Remarks,
                    ISNULL(H.IsActive,0) IsActive,
                    FORMAT(H.CreatedAt,'yyyy-MM-dd HH:mm') CreatedAt,
                    ISNULL(H.CreatedBy,'') CreatedBy
                FROM QuestionSetHeaders H
                WHERE 1=1";
                query = ApplyConditions(query, fields, values, false);

                DataTable dt = new();
                SqlDataAdapter adp = CreateAdapter(query, conn, tran);
                adp.SelectCommand = ApplyParameters(adp.SelectCommand, fields, values);
                adp.Fill(dt);

                result.Status = "Success";
                result.Message = "Report data retrieved.";
                result.DataVM = dt;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== MULTIPLE POST ===========================
        public async Task<ResultVM> MultiplePost(CommonVM vm, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                string inClause = string.Join(", ", vm.IDs.Select((id, i) => $"@Id{i}"));
                string query = $@"
                UPDATE QuestionSetHeaders
                SET IsActive = 1,
                    LastUpdateBy = @ModifyBy,
                    LastUpdateAt = GETDATE(),
                    LastUpdateFrom = @ModifyFrom
                WHERE Id IN ({inClause}) AND IsActive = 0";

                using (SqlCommand cmd = new(query, conn, tran))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    cmd.Parameters.AddWithValue("@ModifyBy", vm.ModifyBy ?? "System");
                    cmd.Parameters.AddWithValue("@ModifyFrom", vm.ModifyFrom ?? "Local");

                    int rows = await cmd.ExecuteNonQueryAsync();
                    result.Status = rows > 0 ? "Success" : "Fail";
                    result.Message = rows > 0 ? "Data posted successfully." : "Already active.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== GET DETAIL GRID BY HEADER ===========================
        public async Task<ResultVM> GetQuestionSetDetailDataById(GridOptions options, int headerId, SqlConnection conn, SqlTransaction tran)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                var data = new GridEntity<QuestionSetDetailVM>();

                string query = @"
                -- Count query
                SELECT COUNT(D.Id) totalcount 
                FROM QuestionSetDetails D 
                LEFT JOIN QuestionHeaders Q ON D.QuestionHeaderId = Q.Id
                WHERE D.QuestionSetHeaderId = @headerId
                " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<QuestionSetDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

                -- Data query
                SELECT * FROM (
                    SELECT 
                    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") rowindex,
                    ISNULL(D.Id,0) Id,
                    ISNULL(D.QuestionSetHeaderId,0) QuestionSetHeaderId,
                    ISNULL(D.QuestionHeaderId,0) QuestionHeaderId,
                    ISNULL(Q.QuestionText,'') QuestionText,
                    ISNULL(Q.QuestionType,'') QuestionType,
                    ISNULL(D.QuestionMark,0) QuestionMark
                    FROM QuestionSetDetails D
                    LEFT JOIN QuestionHeaders Q ON D.QuestionHeaderId = Q.Id
                    WHERE D.QuestionSetHeaderId = @headerId
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<QuestionSetDetailVM>.FilterCondition(options.filter) + ")" : "") + @"
                ) A
                WHERE rowindex > @skip AND (@take=0 OR rowindex<=@take)
                ";

                query = query.Replace("@headerId", headerId.ToString());
                data = KendoGrid<QuestionSetDetailVM>.GetGridData_CMD(options, query, "D.Id");

                result.Status = "Success";
                result.Message = "Detail grid retrieved successfully.";
                result.DataVM = data;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        // =========================== INSERT QUESTION SET QUESTION ===========================
        public async Task<ResultVM> InsertQuestionSetQuestion(QuestionSetQuestionVM vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO QuestionSetQuestions
        (
            QuestionSetHeaderId, QuestionHeaderId
        )
        VALUES
        (
            @QuestionSetHeaderId, @QuestionHeaderId
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@QuestionSetHeaderId", vm.QuestionSetHeaderId);
                    cmd.Parameters.AddWithValue("@QuestionHeaderId", vm.QuestionHeaderId);

                    var idObj = await cmd.ExecuteScalarAsync();
                    vm.Id = Convert.ToInt32(idObj);

                    result.Status = "Success";
                    result.Message = "Question linked to set successfully.";
                    result.Id = vm.Id.ToString();
                    result.DataVM = vm;
                }
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        // =========================== QUESTION SET QUESTION LIST ===========================
        public async Task<ResultVM> QuestionSetQuestionList(string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
            SELECT
                ISNULL(QSQ.Id, 0) Id,
                ISNULL(QSQ.QuestionSetHeaderId, 0) QuestionSetHeaderId,
                ISNULL(QSQ.QuestionHeaderId, 0) QuestionHeaderId,
                ISNULL(QH.QuestionText, '') QuestionText,
                ISNULL(QH.QuestionType, '') QuestionType,
                ISNULL(QH.QuestionMark, 0) QuestionMark
            FROM QuestionSetQuestions QSQ
            LEFT OUTER JOIN QuestionHeaders QH ON QSQ.QuestionHeaderId = QH.Id
            WHERE 1 = 1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND QSQ.Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
                adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                adapter.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Question set details retrieved successfully.";
                result.DataVM = dataTable;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }


        // =========================== GRID DATA BY QUESTION ===========================
        public async Task<ResultVM> GetQuestionSetGridDataByQuestion(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            try
            {
                var data = new GridEntity<QuestionSetHeaderVM>();

                string sqlQuery = @"
        -- Count query
        SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM QuestionSetQuestions D
        LEFT OUTER JOIN QuestionSetHeaders H ON D.QuestionSetHeaderId = H.Id
        LEFT OUTER JOIN QuestionHeaders QH ON D.QuestionHeaderId = QH.Id
        WHERE 1 = 1";

                // Add filters
                sqlQuery += options.filter.Filters.Count > 0
                    ? " AND (" + GridQueryBuilder<QuestionSetHeaderVM>.FilterCondition(options.filter) + ")"
                    : "";

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
        -- Data query with pagination and sorting
        SELECT * FROM (
            SELECT 
                ROW_NUMBER() OVER(ORDER BY " +
                        (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,
                ISNULL(D.QuestionSetHeaderId,0) QuestionSetHeaderId,
                ISNULL(D.QuestionHeaderId,0) QuestionHeaderId,
                ISNULL(H.Name,'') QuestionSetName,
                ISNULL(QH.QuestionText,'') QuestionText,
                ISNULL(QH.QuestionType,'') QuestionType,
                ISNULL(QH.QuestionMark,0) QuestionMark,
                FORMAT(H.CreatedAt, 'yyyy-MM-dd HH:mm') CreatedAt
            FROM QuestionSetQuestions D
            LEFT OUTER JOIN QuestionSetHeaders H ON D.QuestionSetHeaderId = H.Id
            LEFT OUTER JOIN QuestionHeaders QH ON D.QuestionHeaderId = QH.Id
            WHERE 1=1";

                sqlQuery += options.filter.Filters.Count > 0
                    ? " AND (" + GridQueryBuilder<QuestionSetHeaderVM>.FilterCondition(options.filter) + ")"
                    : "";

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
        ) AS a
        WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)";

                // Execute
                data = KendoGrid<QuestionSetHeaderVM>.GetTransactionalQuestionGridData_CMD(
                    options,
                    sqlQuery,
                    "D.Id",
                    conditionalFields,
                    conditionalValues
                );

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }

    }
}
