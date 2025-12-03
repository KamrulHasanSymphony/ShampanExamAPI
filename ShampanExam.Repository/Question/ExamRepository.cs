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
    public class ExamRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(ExamVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                INSERT INTO Exams
                (
                    Code, Name, Date, Time, Duration, TotalMark, GradeId, Remarks, IsExamByQuestionSet, 
                    QuestionSetId, ExamineeGroupId, IsActive, IsArchive, CreatedBy, CreatedFrom, CreatedAt
                )
                VALUES
                (
                    @Code, @Name, @Date, @Time, @Duration, @TotalMark, @GradeId, @Remarks, @IsExamByQuestionSet, 
                    @QuestionSetId, @ExamineeGroupId, @IsActive, @IsArchive, @CreatedBy, @CreatedFrom, GETDATE()
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", vm.Date ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Time", vm.Time ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Duration", vm.Duration);
                    cmd.Parameters.AddWithValue("@TotalMark", vm.TotalMark);
                    cmd.Parameters.AddWithValue("@GradeId", vm.GradeId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", vm.Remarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsExamByQuestionSet", vm.IsExamByQuestionSet);
                    cmd.Parameters.AddWithValue("@QuestionSetId", vm.QuestionSetId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExamineeGroupId", vm.ExamineeGroupId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                result.Status = "Success";
                result.Message = "Exam inserted successfully.";
                result.Id = vm.Id.ToString();
                result.DataVM = vm;

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }

        // Update Method
        public async Task<ResultVM> Update(ExamVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = vm.Id.ToString(), DataVM = vm };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                UPDATE Exams
                SET 
                    Code = @Code,
                    Name = @Name,
                    Date = @Date,
                    Time = @Time,
                    Duration = @Duration,
                    TotalMark = @TotalMark,
                    GradeId = @GradeId,
                    Remarks = @Remarks,
                    IsExamByQuestionSet = @IsExamByQuestionSet,
                    QuestionSetId = @QuestionSetId,
                    ExamineeGroupId = @ExamineeGroupId,
                    IsActive = @IsActive,
                    LastUpdateBy = @LastUpdateBy,
                    LastUpdateFrom = @LastUpdateFrom,
                    LastUpdateAt = GETDATE()
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", vm.Date ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Time", vm.Time ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Duration", vm.Duration);
                    cmd.Parameters.AddWithValue("@TotalMark", vm.TotalMark);
                    cmd.Parameters.AddWithValue("@GradeId", vm.GradeId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", vm.Remarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsExamByQuestionSet", vm.IsExamByQuestionSet);
                    cmd.Parameters.AddWithValue("@QuestionSetId", vm.QuestionSetId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExamineeGroupId", vm.ExamineeGroupId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Exam updated successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were updated.");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }

        // Delete (Archive) Method
        public async Task<ResultVM> Delete(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = string.Join(",", vm.IDs) };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string inClause = string.Join(", ", vm.IDs.Select((id, index) => $"@Id{index}"));

                string query = $@"
                UPDATE Exams
                SET IsArchive = 1, IsActive = 0,
                    LastUpdateBy = @LastUpdateBy,
                    LastUpdateFrom = @LastUpdateFrom,
                    LastUpdateAt = GETDATE()
                WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }
                    cmd.Parameters.AddWithValue("@LastUpdateBy", vm.ModifyBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom ?? (object)DBNull.Value);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Exam deleted successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were deleted.");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }

        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null,
            SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dt = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                SELECT 
                    ISNULL(M.Id,0) AS Id,
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.Name, '') AS Name,
                    ISNULL(FORMAT(M.Date, 'yyyy-MM-dd'), '') AS Date,
                    ISNULL(M.Time, '') AS Time,
                    ISNULL(M.Duration, 0) AS Duration,
                    ISNULL(M.TotalMark, 0) AS TotalMark,
                    ISNULL(M.GradeId, 0) AS GradeId,
                    ISNULL(M.Remarks, '') AS Remarks,
                    ISNULL(M.IsExamByQuestionSet, 0) AS IsExamByQuestionSet,
                    ISNULL(M.QuestionSetId, 0) AS QuestionSetId,
                    ISNULL(M.ExamineeGroupId, 0) AS ExamineeGroupId,
                    ISNULL(M.IsActive, 0) AS IsActive,
                    ISNULL(M.IsArchive, 0) AS IsArchive,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedAt, 'yyyy-MM-dd HH:mm'), '') AS CreatedAt,
                    ISNULL(M.LastUpdateBy, '') AS LastUpdateBy,
                    ISNULL(FORMAT(M.LastUpdateAt, 'yyyy-MM-dd HH:mm'), '') AS LastUpdateAt
                FROM Exams M
                WHERE 1=1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
                adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);
                adapter.Fill(dt);

                var list = dt.AsEnumerable().Select(row => new ExamVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    Date = row.Field<string>("Date"),
                    Time = row.Field<TimeSpan?>("Time"),
                    Duration = row.Field<int>("Duration"),
                    TotalMark = row.Field<int>("TotalMark"),
                    GradeId = row.Field<int?>("GradeId"),
                    Remarks = row.Field<string>("Remarks"),
                    IsExamByQuestionSet = row.Field<bool>("IsExamByQuestionSet"),
                    QuestionSetId = row.Field<int?>("QuestionSetId"),
                    ExamineeGroupId = row.Field<int?>("ExamineeGroupId"),
                    IsActive = row.Field<bool>("IsActive"),
                    IsArchive = row.Field<bool>("IsArchive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedAt = row.Field<string>("CreatedAt"),
                    LastUpdateBy = row.Field<string>("LastUpdateBy"),
                    LastUpdateAt = row.Field<string>("LastUpdateAt")
                }).ToList();

                result.Status = "Success";
                result.Message = "Exams retrieved successfully.";
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

        // GetProcessedData Mathod
        public async Task<ResultVM> GetProcessedData( string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                int examId = 0;
                if (conditionalFields != null && conditionalValues != null)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        if (conditionalFields[i].Equals("Id", StringComparison.OrdinalIgnoreCase))
                        {
                            int.TryParse(conditionalValues[i], out examId);
                            break;
                        }
                    }
                }

                if (examId <= 0)
                {
                    result.Message = "ExamId not found in parameters.";
                    return result;
                }
                int groupId = 0;
                if (conditionalFields != null && conditionalValues != null)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        if (conditionalFields[i].Equals("ExamineeGroupId", StringComparison.OrdinalIgnoreCase))
                        {
                            int.TryParse(conditionalValues[i], out groupId);
                            break;
                        }
                    }
                }

                if (groupId <= 0)
                {
                    result.Message = "group is not found in parameters.";
                    return result;
                }
                int setId = 0;
                if (conditionalFields != null && conditionalValues != null)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        if (conditionalFields[i].Equals("QuestionSetId", StringComparison.OrdinalIgnoreCase))
                        {
                            int.TryParse(conditionalValues[i], out setId);
                            break;
                        }
                    }
                }

                if (setId <= 0)
                {
                    result.Message = "group is not found in parameters.";
                    return result;
                }

                using (SqlCommand cmd = new SqlCommand("InsertExamQuestionHeaders", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    //cmd.Parameters.AddWithValue("@ExamineeGroupId", groupId);
                    //cmd.Parameters.AddWithValue("@QuestionSetId", setId);

                    await cmd.ExecuteNonQueryAsync();
                }

                //using (SqlCommand cmd = new SqlCommand("InsertExamQuestionHeaders", conn, transaction))
                //{
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Parameters.AddWithValue("@ExamId", examId);
                //    cmd.Parameters.AddWithValue("@ExamineeGroupId", groupId);
                //    cmd.Parameters.AddWithValue("@QuestionSetId", setId);
                //    await cmd.ExecuteNonQueryAsync();
                //}

                result.Status = "Success";
                result.Message = "Exam data processed and fetched successfully.";
                result.DataVM = result;

            }
            catch (Exception ex)
            {
                result.Status = "Fail";
                result.Message = "Error occurred while processing exam data.";
                result.ExMessage = ex.Message;
            }

            return result;
        }
        public async Task<ResultVM> ListOfProcessedData(string[] conditionalFields,string[] conditionalValues,PeramModel vm = null,SqlConnection conn = null,SqlTransaction transaction = null)
        {
            ResultVM resultList = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                int examId = 0;
                if (conditionalFields != null && conditionalValues != null)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        if (conditionalFields[i].Equals("Id", StringComparison.OrdinalIgnoreCase))
                        {
                            int.TryParse(conditionalValues[i], out examId);
                            break;
                        }
                    }
                }

                if (examId <= 0)
                {
                    resultList.Message = "ExamId not found in parameters.";
                    return resultList;
                }

                string sqlQuery = @"
            SELECT * FROM ExamQuestionHeaders WHERE ExamId = @ExamId;
            SELECT * FROM ExamQuestionOptionDetails WHERE ExamId = @ExamId;
            SELECT * FROM ExamQuestionShortDetails WHERE ExamId = @ExamId;
            SELECT * FROM ExamExaminees WHERE ExamId = @ExamId;
        ";

                var ds = new DataSet();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }

                // ✅ Create a single ExamVM and fill its lists
                var examVm = new ExamVM();

                // --- Table 1: ExamQuestionHeaders
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    examVm.examQuestionHeaderList = ds.Tables[0].AsEnumerable().Select(r => new ExamQuestionHeaderVM
                    {
                        Id = r.Field<int>("Id"),
                        ExamId = r.Field<int>("ExamId"),
                        ExamineeId = r.Field<int>("ExamineeId"),
                        QuestionHeaderId = r.Field<int>("QuestionHeaderId"),
                        QuestionText = r.Field<string?>("QuestionText"),
                        QuestionType = r.Field<string?>("QuestionType"),
                        QuestionMark = r.Field<int?>("QuestionMark"),
                        MarkObtain = r.Field<decimal?>("MarkObtain")
                    }).ToList();
                }

                // --- Table 2: ExamQuestionOptionDetails
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    examVm.examQuestionOptionDetailList = ds.Tables[1].AsEnumerable().Select(r => new ExamQuestionOptionDetailVM
                    {
                        Id = r.Field<int>("Id"),
                        ExamId = r.Field<int>("ExamId"),
                        ExamQuestionHeaderId = r.Field<int>("ExamQuestionHeaderId"),
                        QuestionHeaderId = r.Field<int>("QuestionHeaderId"),
                        QuestionOptionDetailId = r.Field<int>("QuestionOptionDetailId"),
                        QuestionOption = r.Field<string?>("QuestionOption"),
                        QuestionAnswer = r.Field<bool?>("QuestionAnswer"),
                        ExamineeAnswer = r.Field<bool?>("ExamineeAnswer")
                    }).ToList();
                }

                // --- Table 3: ExamQuestionShortDetails
                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    examVm.examQuestionShortDetailList = ds.Tables[2].AsEnumerable().Select(r => new ExamQuestionShortDetailVM
                    {
                        Id = r.Field<int>("Id"),
                        ExamId = r.Field<int>("ExamId"),
                        ExamQuestionHeaderId = r.Field<int>("ExamQuestionHeaderId"),
                        QuestionHeaderId = r.Field<int>("QuestionHeaderId"),
                        QuestionShortDetailId = r.Field<int>("QuestionShortDetailId"),
                        QuestionAnswer = r.Field<string?>("QuestionAnswer"),
                        ExamineeAnswer = r.Field<string?>("ExamineeAnswer")
                    }).ToList();
                }

                // --- Table 4: ExamExaminees
                if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    examVm.examExamineeList = ds.Tables[3].AsEnumerable().Select(r => new ExamExamineeVM
                    {
                        Id = r.Field<int>("Id"),
                        ExamId = r.Field<string>("ExamId"),
                        ExamineeId = r.Field<int>("ExamineeId")
                    }).ToList();
                }

                resultList.Status = "Success";
                resultList.Message = "Exam processed data fetched successfully.";
                resultList.DataVM = examVm; 
            }
            catch (Exception ex)
            {
                resultList.Status = "Fail";
                resultList.Message = "Error occurred while processing exam data.";
                resultList.ExMessage = ex.Message;
            }

            return resultList;
        }




        // GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                var data = new GridEntity<ExamVM>();

                string sqlQuery = @"
        -- Count
        SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM Exams H
        WHERE H.IsArchive != 1
        " + (options.filter.Filters.Count > 0
                        ? " AND (" + GridQueryBuilder<ExamVM>.FilterCondition(options.filter) + ")"
                        : "") + @"

        -- Data
        SELECT *
        FROM (
            SELECT ROW_NUMBER() OVER(ORDER BY " +
                        (options.sort.Count > 0
                            ? "H." + options.sort[0].field + " " + options.sort[0].dir
                            : "H.Id DESC") + @") AS rowindex,
                   ISNULL(H.Id,0) AS Id,
                   ISNULL(H.Code, '') AS Code,
                   ISNULL(H.Name, '') AS Name,
                   ISNULL(H.Date, '') AS Date,
                   ISNULL(H.Time, '') AS Time,
                   ISNULL(H.Duration, 0) AS Duration,
                   ISNULL(H.TotalMark, 0) AS TotalMark,
                   ISNULL(H.GradeId, 0) AS GradeId,
                   ISNULL(H.Remarks, '') AS Remarks,
                   ISNULL(H.IsExamByQuestionSet, 0) AS IsExamByQuestionSet,
                   ISNULL(H.QuestionSetId, 0) AS QuestionSetId,
                   ISNULL(H.ExamineeGroupId, 0) AS ExamineeGroupId,
                   CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                   ISNULL(H.CreatedBy, '') AS CreatedBy,
                   ISNULL(FORMAT(H.CreatedAt, 'yyyy-MM-dd HH:mm'), '') AS CreatedAt,
                   ISNULL(H.LastUpdateBy, '') AS LastUpdateBy,
                   ISNULL(FORMAT(H.LastUpdateAt, 'yyyy-MM-dd HH:mm'), '') AS LastUpdateAt
            FROM Exams H
            WHERE H.IsArchive != 1
            " + (options.filter.Filters.Count > 0
                            ? " AND (" + GridQueryBuilder<ExamVM>.FilterCondition(options.filter) + ")"
                            : "") + @"
        ) AS a
        WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)";

                data = KendoGrid<ExamVM>.GetGridDataQuestions_CMD(options, sqlQuery, "H.Id");

                result.Status = "Success";
                result.Message = "Exams grid data retrieved successfully.";
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

    }
}
