using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.Exam;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ExamVM = ShampanExam.ViewModel.QuestionVM.ExamVM;

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
                    QuestionSetId, ExamineeGroupId,ExamType, IsActive, IsArchive, CreatedBy, CreatedFrom, CreatedAt
                )
                VALUES
                (
                    @Code, @Name, @Date, @Time, @Duration, @TotalMark, @GradeId, @Remarks, @IsExamByQuestionSet, 
                    @QuestionSetId, @ExamineeGroupId,@ExamType, @IsActive, @IsArchive, @CreatedBy, @CreatedFrom, GETDATE()
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
                    cmd.Parameters.AddWithValue("@ExamType", vm.ExamType ?? (object)DBNull.Value);

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
        public async Task<ResultVM> SelfInsert(ExamVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    QuestionSetId, ExamineeGroupId, IsActive, IsArchive, CreatedBy, CreatedFrom, CreatedAt,ExamType
                )
                VALUES
                (
                    @Code, @Name, @Date, @Time, @Duration, @TotalMark, @GradeId, @Remarks, @IsExamByQuestionSet, 
                    @QuestionSetId, @ExamineeGroupId, @IsActive, @IsArchive, @CreatedBy, @CreatedFrom, GETDATE(),@ExamType
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
                    cmd.Parameters.AddWithValue("@ExamType", "Mock");

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
        public async Task<ResultVM> DetailsInsert(AutomatedExamDetailsVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                INSERT INTO AutomatedExamDetails
                (
                       AutomatedExamId
                      ,SubjectId
                      ,NumberOfQuestion
                      ,QuestionType
                      ,QuestionMark
                )
                VALUES
                (
                    @AutomatedExamId, @SubjectId, @NumberOfQuestion, @QuestionType, @QuestionMark
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@AutomatedExamId", vm.Id );
                    cmd.Parameters.AddWithValue("@SubjectId", vm.SubjectId ?? (object)DBNull.Value);

                    int? numberOfQuestion = vm.NumberOfQuestion?? vm.SingleOptionNo?? vm.MultiOptionNo;

                    cmd.Parameters.AddWithValue("@NumberOfQuestion", numberOfQuestion.HasValue ? (object)numberOfQuestion.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuestionType", vm.QuestionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuestionMark", vm.QuestionMark);
                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                result.Status = "Success";
                result.Message = "Exam Details inserted successfully.";
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

        //       // List Method
        //       public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null,
        //           SqlConnection conn = null, SqlTransaction transaction = null)
        //       {
        //           DataTable dt = new DataTable();
        //           ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //           try
        //           {
        //               if (conn == null) throw new Exception("Database connection failed!");

        //               string query = @"
        //               SELECT 
        //                   ISNULL(M.Id,0) AS Id,
        //                   ISNULL(M.Code, '') AS Code,
        //                   ISNULL(M.Name, '') AS Name,
        //                   ISNULL(FORMAT(M.Date, 'yyyy-MM-dd'), '') AS Date,
        //                   ISNULL(M.Time, '') AS Time,
        //                   ISNULL(M.Duration, 0) AS Duration,
        //                   ISNULL(M.TotalMark, 0) AS TotalMark,
        //                   ISNULL(M.GradeId, 0) AS GradeId,
        //                   ISNULL(M.Remarks, '') AS Remarks,
        //                   ISNULL(M.IsExamByQuestionSet, 0) AS IsExamByQuestionSet,
        //                   ISNULL(M.QuestionSetId, 0) AS QuestionSetId,
        //                   ISNULL(M.ExamineeGroupId, 0) AS ExamineeGroupId,
        //                   ISNULL(M.IsActive, 0) AS IsActive,
        //                   ISNULL(M.IsArchive, 0) AS IsArchive,
        //                   ISNULL(M.CreatedBy, '') AS CreatedBy,
        //                   ISNULL(FORMAT(M.CreatedAt, 'yyyy-MM-dd HH:mm'), '') AS CreatedAt,
        //                   ISNULL(M.LastUpdateBy, '') AS LastUpdateBy,
        //                   ISNULL(FORMAT(M.LastUpdateAt, 'yyyy-MM-dd HH:mm'), '') AS LastUpdateAt
        //               FROM Exams M
        //               WHERE 1=1";

        //               query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //               SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
        //               adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);
        //               adapter.Fill(dt);

        //               var list = dt.AsEnumerable().Select(row => new ExamVM
        //               {
        //                   Id = row.Field<int>("Id"),
        //                   Code = row.Field<string>("Code"),
        //                   Name = row.Field<string>("Name"),
        //                   Date = row.Field<string>("Date"),
        //                   Time = row.Field<TimeSpan?>("Time"),
        //                   Duration = row.Field<int>("Duration"),
        //                   TotalMark = row.Field<int>("TotalMark"),
        //                   GradeId = row.Field<int?>("GradeId"),
        //                   Remarks = row.Field<string>("Remarks"),
        //                   IsExamByQuestionSet = row.Field<bool>("IsExamByQuestionSet"),
        //                   QuestionSetId = row.Field<int?>("QuestionSetId"),
        //                   ExamineeGroupId = row.Field<int?>("ExamineeGroupId"),
        //                   IsActive = row.Field<bool>("IsActive"),
        //                   IsArchive = row.Field<bool>("IsArchive"),
        //                   CreatedBy = row.Field<string>("CreatedBy"),
        //                   CreatedAt = row.Field<string>("CreatedAt"),
        //                   LastUpdateBy = row.Field<string>("LastUpdateBy"),
        //                   LastUpdateAt = row.Field<string>("LastUpdateAt")
        //               }).ToList();



        //               if (list.Count>0)
        //               {
        //                   string newquery = @"
        //               SELECT  [Id]
        //     ,[AutomatedExamId]
        //     ,[SubjectId]
        //     ,[NumberOfQuestion]
        //     ,[QuestionType]
        //     ,[QuestionMark]
        //From [AutomatedExamDetails]";

        //                   query = ApplyConditions(newquery, new[] { "[AutomatedExamId]" }, conditionalValues, false);

        //                    adapter = CreateAdapter(newquery, conn, transaction);
        //                   adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);
        //                   adapter.Fill(dt);

        //                   var listt = dt.AsEnumerable().Select(row => new AutomatedExamDetailsVM
        //                   {
        //                       Id = row.Field<int>("Id"),
        //                       AutomatedExamId = row.Field<string>("AutomatedExamId"),
        //                       SubjectId = row.Field<int>("SubjectId"),
        //                       NumberOfQuestion = row.Field<int>("NumberOfQuestion"),
        //                       QuestionType = row.Field<string>("QuestionType"),
        //                       QuestionMark = row.Field<int>("QuestionMark"),

        //                   }).ToList();
        //                   list.automatedExamDetailList= listt
        //               }
        //               result.Status = "Success";
        //               result.Message = "Exams retrieved successfully.";
        //               result.DataVM = list;

        //               return result;
        //           }
        //           catch (Exception ex)
        //           {
        //               result.Message = ex.Message;
        //               result.ExMessage = ex.ToString();
        //               return result;
        //           }
        //       }


        //List Method
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

                if (list.Count > 0)
                {
                    string newquery = @"
                    SELECT AutomatedExamDetails.Id,
                           [AutomatedExamId],
                           [SubjectId],
	                       S.Name SubjectName,
                           [NumberOfQuestion],
                           [QuestionType],
                           [QuestionMark]
                    FROM [AutomatedExamDetails]
                    LEFT OUTER JOIN QuestionSubjects S ON AutomatedExamDetails.SubjectId = S.Id
                    WHERE 1=1";

                    newquery = ApplyConditions(newquery, new[] { "AutomatedExamId" }, conditionalValues, false);

                    DataTable dt2 = new DataTable();
                    adapter = CreateAdapter(newquery, conn, transaction);
                    adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, new[] { "AutomatedExamId" }, conditionalValues);
                    adapter.Fill(dt2);

                    var listt = dt2.AsEnumerable().Select(row => new AutomatedExamDetailsVM
                    {
                        Id = row.Field<int>("Id"),
                        AutomatedExamId = row.Field<int>("AutomatedExamId"),
                        SubjectId = row.Field<int>("SubjectId"),
                        NumberOfQuestion = row.Field<int>("NumberOfQuestion"),
                        QuestionType = row.Field<string>("QuestionType"),
                        SubjectName = row.Field<string>("SubjectName"),
                        QuestionMark = row.Field<decimal>("QuestionMark"),
                    }).ToList();

                    // Assign the details to each exam (if needed for all exams)
                    foreach (var exam in list)
                    {
                        exam.automatedExamDetailList = listt;
                    }

                    string newexamineequery = @"
                    Select
                    ISNULL(E.Id,0) Id,
                    ISNULL(E.ExamineeId,0) ExamineeId,
                    ISNULL(Ex.Name,'') Name
                    from ExamExaminees E
                    LEFT OUTER JOIN Examinees Ex ON E.ExamineeId = Ex.Id
                    WHERE 1=1 AND E.ExamId = @ExamId";

                    //newquery = ApplyConditions(newquery, new[] { "ExamId" }, conditionalValues, false);

                    DataTable dt3 = new DataTable();

                    adapter = CreateAdapter(newexamineequery, conn, transaction);
                    adapter.SelectCommand.Parameters.AddWithValue("@ExamId", conditionalValues[0]);
                    adapter.Fill(dt3);

                    var listexaminee = dt3.AsEnumerable().Select(row => new ExamExamineeVM
                    {
                        Id = row.Field<int>("Id"),
                        ExamineeId = row.Field<int>("ExamineeId"),
                        Name = row.Field<string>("Name")
                    }).ToList();


                    // Assign the details to each exam (if needed for all exams)
                    foreach (var exam in list)
                    {
                        exam.examExamineeList = listexaminee;
                    }

                    string newquestionquery = @"
                    Select
                    ISNULL(Id,0) Id,
                    ISNULL(ExamId,0) ExamId,
                    ISNULL(ExamineeId,0) ExamineeId,
                    ISNULL(IsExamSubmitted,0) IsExamSubmitted,
                    ISNULL(IsExamMarksSubmitted,0) IsExamMarksSubmitted,
                    ISNULL(QuestionHeaderId,0) QuestionHeaderId,
                    ISNULL(QuestionText,'') QuestionText,
                    ISNULL(QuestionType,'') QuestionType,
                    ISNULL(QuestionMark,'') QuestionMark

                    from ExamQuestionHeaders 
                    WHERE ExamId = @ExamId ";

                    //newquery = ApplyConditions(newquery, new[] { "ExamId" }, conditionalValues, false);

                    DataTable dt4 = new DataTable();
                    adapter = CreateAdapter(newquestionquery, conn, transaction);
                    adapter.SelectCommand.Parameters.AddWithValue("@ExamId", conditionalValues[0]);
                    adapter.Fill(dt4);

                    var listquestion = dt4.AsEnumerable().Select(row => new ExamQuestionHeaderVM
                    {
                        Id = row.Field<int>("Id"),
                        ExamId = row.Field<int>("ExamId"),
                        ExamineeId = row.Field<int>("ExamineeId"),
                        QuestionHeaderId = row.Field<int>("QuestionHeaderId"),
                        QuestionText = row.Field<string>("QuestionText"),
                        QuestionType = row.Field<string>("QuestionType"),
                        QuestionMark = row.Field<int>("QuestionMark")
                    }).ToList();

                    // Assign the details to each exam (if needed for all exams)
                    foreach (var exam in list)
                    {
                        exam.examQuestionHeaderList = listquestion;
                    }

                }

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
        public async Task<ResultVM> GetExamInfoReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                // Define base query for feeding plan report
                string query = @"
         SELECT 
ISNULL(EX.Id,0) AS Id, 
ISNULL(EX.Name,'') AS ExamName, 
ISNULL(EX.Code,'') AS ExamCode,
ISNULL(EX.TotalMark,'') AS TotalMark,
ISNULL(EX.Duration,'') AS Duration, 
ISNULL(EX.Date,'') AS ExamDate
FROM Exams EX
WHERE 1 = 1
              ";

                // Apply dynamic filter conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                // Initialize SqlDataAdapter for executing the query
                SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
                adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);

                adapter.Fill(dataTable);

                var model = dataTable.AsEnumerable().Select(row => new ExamReportHeaderVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ExamCode = row["ExamCode"]?.ToString(),
                    ExamName = row["ExamName"]?.ToString(),
                    TotalMark = row["TotalMark"]?.ToString(),
                    Duration = row["Duration"]?.ToString(),
                    ExamDate = row["ExamDate"]?.ToString()
                }).ToList();

                // Load details
                var detailResult = DetailsList(new[] { "ExamId" }, conditionalValues, vm, conn, transaction);

                if (detailResult.Status == "Success" && detailResult.DataVM is DataTable dts)
                {
                    string json = JsonConvert.SerializeObject(dts);
                    var details = JsonConvert.DeserializeObject<List<ExamReportDetailVM>>(json);

                    if (model.Any())
                        model.First().examReportDetailList = details;
                }

                result.Status = "Success";
                result.Message = "Exam report data retrieved successfully.";
                result.DataVM = model;
                return result;
            }
            catch (Exception ex)
            {
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }


        #region DetailsList
        public ResultVM DetailsList(string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail" };
            DataTable dataTable = new DataTable();

            try
            {
                string query = @"
                SELECT  
----master---
    Exams.Code AS ExamCode,
    Exams.Name AS ExamName,
    Exams.Duration,
    Exams.Date,
	----master---

		----Details---
    Examinees.Name AS ExamineeName,
    Examinees.MobileNo AS ExamineeMobileNo,
    Exams.TotalMark,
    Examinees.MarkObtain,
    -- ⭐ Grade from GradeDetails
    GradeDetails.Grade,
    -- (Optional) Percentage
    ((Examinees.MarkObtain / Exams.TotalMark) * 100) AS Percentage
			----Details---

FROM  
(
    SELECT  
        ExamId,
        Examinees.Name,
        Examinees.MobileNo,
        ExamineeId,
        SUM(MarkObtain) AS MarkObtain  
    FROM ExamQuestionHeaders
    LEFT JOIN Examinees 
        ON ExamQuestionHeaders.ExamineeId = Examinees.Id
    WHERE IsExamMarksSubmitted = 1
    GROUP BY ExamId, Examinees.Name, Examinees.MobileNo, ExamineeId
) AS Examinees

LEFT JOIN Exams 
    ON Exams.Id = Examinees.ExamId

LEFT JOIN Grades 
    ON Exams.GradeId = Grades.Id

LEFT JOIN GradeDetails 
    ON GradeDetails.GradeId = Grades.Id
    AND ((Examinees.MarkObtain / Exams.TotalMark) * 100) 
        BETWEEN GradeDetails.MinPercentage AND GradeDetails.MaxPercentage


-----Perameter-----
		where 1=1

		-----Perameter-----";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter da = CreateAdapter(query, conn, transaction);
                da.SelectCommand = ApplyParameters(da.SelectCommand, conditionalFields, conditionalValues);
                da.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Issue Details retrieved successfully.";
                result.DataVM = dataTable;
            }
            catch (Exception ex)
            {
                result.Message = "Error in DetailsList.";
                result.ExMessage = ex.ToString();
            }
            return result;
        }
        #endregion



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
        WHERE H.IsArchive != 1 AND H.IsActive = 1 AND H.ExamineeGroupId != 0
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
            WHERE H.IsArchive != 1 AND H.IsActive = 1 AND H.ExamineeGroupId != 0
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

        public async Task<ResultVM> GetRandomProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                int examId = 0, groupId = 0, setId = 0, questionSubjectId = 0, noOfQuestion = 0;
                string questionType = null;

                if (conditionalFields != null && conditionalValues != null)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        switch (conditionalFields[i].ToLower())
                        {
                            case "id":
                                int.TryParse(conditionalValues[i], out examId);
                                break;
                            case "examineegroupid":
                                int.TryParse(conditionalValues[i], out groupId);
                                break;
                            case "questionsubjectid":
                                int.TryParse(conditionalValues[i], out questionSubjectId);
                                break;
                            case "questiontype":
                                questionType = conditionalValues[i];
                                break;
                            case "noofquestion":
                                int.TryParse(conditionalValues[i], out noOfQuestion);
                                break;
                        }
                    }
                }

                // Validation
                //if (examId <= 0) return new ResultVM { Status = "Fail", Message = "ExamId not found." };
                //if (groupId <= 0) return new ResultVM { Status = "Fail", Message = "GroupId not found." };
                //if (setId <= 0) return new ResultVM { Status = "Fail", Message = "QuestionSetId not found." };
                //if (questionSubjectId <= 0) return new ResultVM { Status = "Fail", Message = "QuestionSubjectId not found." };
                //if (string.IsNullOrEmpty(questionType)) return new ResultVM { Status = "Fail", Message = "QuestionType not found." };
                //if (noOfQuestion <= 0) return new ResultVM { Status = "Fail", Message = "NoOfQuestion not found." };

                using (SqlCommand cmd = new SqlCommand("InsertRandomExamQuestionHeaders", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pass all parameters
                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    cmd.Parameters.AddWithValue("@QuestionSubjectId", questionSubjectId);
                    cmd.Parameters.AddWithValue("@QuestionType", questionType);
                    cmd.Parameters.AddWithValue("@NoOfQuestion", noOfQuestion);

                    await cmd.ExecuteNonQueryAsync();
                }

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

        public async Task<ResultVM> GetUserRandomProcessedData( string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                int examId = 0, questionSubjectId = 0, singleOptionNo = 0, multiOptionNo = 0;
                string userId = null;

                if (conditionalFields != null && conditionalValues != null)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        string field = conditionalFields[i].ToLower();
                        string value = conditionalValues[i];

                        switch (field)
                        {
                            case "examid":
                                int.TryParse(value, out examId);
                                break;

                            case "questionsubjectid":
                                int.TryParse(value, out questionSubjectId);
                                break;

                            case "multioptionno":
                                multiOptionNo = 0;
                                if (!string.IsNullOrEmpty(value))
                                    int.TryParse(value, out multiOptionNo);
                                break;

                            case "singleoptionno":
                                singleOptionNo = 0;
                                if (!string.IsNullOrEmpty(value))
                                    int.TryParse(value, out singleOptionNo);
                                break;

                            case "userid":
                                userId = value;
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(userId))
                    return new ResultVM { Status = "Fail", Message = "UserId not found." };

                using (var cmd = new SqlCommand("InsertUserRandomExamQuestionHeaders", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    cmd.Parameters.AddWithValue("@QuestionSubjectId", questionSubjectId);
                    cmd.Parameters.AddWithValue("@MultiOptionNo", multiOptionNo);   
                    cmd.Parameters.AddWithValue("@SingleOptionNo", singleOptionNo); 
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    await cmd.ExecuteNonQueryAsync();
                }

                result.Status = "Success";
                result.Message = "Exam data processed successfully.";
                result.DataVM = new { ExamId = examId, UserId = userId };
            }
            catch (Exception ex)
            {
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }

            return result;
        }




        //public async Task<ResultVM> GetRandomProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //    try
        //    {
        //        int examId = 0;
        //        if (conditionalFields != null && conditionalValues != null)
        //        {
        //            for (int i = 0; i < conditionalFields.Length; i++)
        //            {
        //                if (conditionalFields[i].Equals("Id", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    int.TryParse(conditionalValues[i], out examId);
        //                    break;
        //                }
        //            }
        //        }

        //        if (examId <= 0)
        //        {
        //            result.Message = "ExamId not found in parameters.";
        //            return result;
        //        }
        //        int groupId = 0;
        //        if (conditionalFields != null && conditionalValues != null)
        //        {
        //            for (int i = 0; i < conditionalFields.Length; i++)
        //            {
        //                if (conditionalFields[i].Equals("ExamineeGroupId", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    int.TryParse(conditionalValues[i], out groupId);
        //                    break;
        //                }
        //            }
        //        }

        //        if (groupId <= 0)
        //        {
        //            result.Message = "group is not found in parameters.";
        //            return result;
        //        }
        //        int setId = 0;
        //        if (conditionalFields != null && conditionalValues != null)
        //        {
        //            for (int i = 0; i < conditionalFields.Length; i++)
        //            {
        //                if (conditionalFields[i].Equals("QuestionSetId", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    int.TryParse(conditionalValues[i], out setId);
        //                    break;
        //                }
        //            }
        //        }

        //        if (setId <= 0)
        //        {
        //            result.Message = "group is not found in parameters.";
        //            return result;
        //        }

        //        using (SqlCommand cmd = new SqlCommand("InsertRandomExamQuestionHeaders", conn, transaction))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@ExamId", examId);

        //            await cmd.ExecuteNonQueryAsync();
        //        }

        //        result.Status = "Success";
        //        result.Message = "Exam data processed and fetched successfully.";
        //        result.DataVM = result;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = "Fail";
        //        result.Message = "Error occurred while processing exam data.";
        //        result.ExMessage = ex.Message;
        //    }

        //    return result;
        //}


        public async Task<ResultVM> GetRandomGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                var data = new GridEntity<ExamVM>();

                string filterCondition = options.filter.Filters.Count > 0
                    ? " AND (" + GridQueryBuilder<ExamVM>.FilterCondition(options.filter) + ")"
                    : "";

                string orderBy = options.sort.Count > 0
                    ? "H." + options.sort[0].field + " " + options.sort[0].dir
                    : "H.Id DESC";

                string sqlQuery = $@"
-- Count
SELECT COUNT(DISTINCT H.Id) AS totalcount
FROM Exams H
LEFT JOIN ExamQuestionHeaders Q ON H.Id = Q.ExamId
LEFT JOIN Examinees E ON Q.ExamineeId = E.Id
LEFT JOIN Users U ON E.LogInId = U.Id
WHERE H.IsArchive != 1 
  AND H.IsActive = 1 
  AND H.ExamineeGroupId = 0 
  AND H.ExamType = 'Mock'
  {filterCondition}

-- Data
;WITH ExamCTE AS
(
    SELECT DISTINCT
        ROW_NUMBER() OVER(ORDER BY {orderBy}) AS rowindex,
        ISNULL(H.Id,0) AS Id,
        ISNULL(H.Code,'') AS Code,
        ISNULL(H.Name,'') AS Name,
        H.Date AS Date,
        CAST(Q.ExamineeId AS BIGINT) AS ExamineeId,                  
        ISNULL(U.Name,'') AS ExamineeName,
        CASE WHEN ISNULL(H.IsActive, 0) = 1 
             THEN 'Active' 
             ELSE 'Inactive' 
        END AS Status
    FROM Exams H
    LEFT JOIN ExamQuestionHeaders Q ON H.Id = Q.ExamId
    LEFT JOIN Examinees E ON Q.ExamineeId = E.Id
    LEFT JOIN Users U ON E.LogInId = U.Id
    WHERE H.IsArchive != 1 
      AND H.IsActive = 1 
      AND H.ExamineeGroupId = 0 
      AND H.ExamType = 'Mock'
      {filterCondition}
)
SELECT *
FROM ExamCTE
WHERE rowindex > @skip 
AND (@take = 0 OR rowindex <= @skip + @take);";

                data = KendoGrid<ExamVM>.GetGridDataQuestions_CMD(options, sqlQuery, "Id");

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


        //public async Task<ResultVM> GetRandomGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //    try
        //    {
        //        if (conn == null) throw new Exception("Database connection failed!");

        //        var data = new GridEntity<ExamVM>();

        //        string sqlQuery = @"
        //        -- Count
        //        SELECT COUNT(DISTINCT H.Id) AS totalcount
        //        FROM Exams H
        //    left outer join ExamQuestionHeaders Q ON H.Id = Q.ExamId
        //    --LEFT OUTER JOIN ExamExaminees E ON H.Id = E.ExamId
        //    LEFT OUTER JOIN Examinees E ON Q.ExamineeId = E.Id
        //    LEFT OUTER JOIN Users U ON E.Id = U.Id
        //    WHERE H.IsArchive != 1 
        //      AND H.IsActive = 1 
        //      AND H.ExamineeGroupId = 0 
        //      AND (H.ExamType = 'Mock' OR H.ExamType IS NOT NULL)
        //        ";

        //        // ✅ Apply User filter only if UserId is NOT "erp"
        //        if (!string.IsNullOrEmpty(options.vm.UserId) && options.vm.UserId.ToLower() != "erp")
        //        {
        //            sqlQuery += " AND U.Name = '" + options.vm.UserId.Replace("'", "''") + "'";
        //        }

        //        sqlQuery += @"
        //        -- Data
        //            SELECT DISTINCT
        //            ISNULL(H.Id,0) AS Id,
        //            ISNULL(H.Code,'') AS Code,
        //            ISNULL(H.Name,'') AS Name,
        //            ISNULL(H.Date,'') AS Date,
        //            CAST(Q.ExamineeId AS BIGINT) AS ExamineeId,                  
        //            ISNULL(U.Name,'') AS ExamineeName,
        //            CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status
        //         FROM Exams H
        //        left outer join ExamQuestionHeaders Q ON H.Id = Q.ExamId
        //        --LEFT OUTER JOIN ExamExaminees E ON H.Id = E.ExamId
        //        LEFT OUTER JOIN Examinees E ON Q.ExamineeId = E.Id
        //        LEFT OUTER JOIN Users U ON E.LogInId = U.Id
        //        WHERE H.IsArchive != 1 
        //          AND H.IsActive = 1 
        //          AND H.ExamineeGroupId = 0 
        //          AND (H.ExamType = 'Mock' OR H.ExamType IS NOT NULL)
        //        ";

        //        // ✅ Same logic for data query
        //        if (!string.IsNullOrEmpty(options.vm.UserId) && options.vm.UserId.ToLower() != "erp")
        //        {
        //            sqlQuery += " AND U.Name = '" + options.vm.UserId.Replace("'", "''") + "'";
        //        }

        //        data = KendoGrid<ExamVM>.GetGridDataQuestions_CMD(sqlQuery, "H.Id");

        //        result.Status = "Success";
        //        result.Message = "Exams grid data retrieved successfully.";
        //        result.DataVM = data;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //}


        public async Task<ResultVM> ExamineeInsert(ExamExamineeVM examinee, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                 INSERT INTO ExamExaminees  (ExamId, ExamineeId, CreatedBy, CreatedFrom, CreatedAt)
                VALUES
                (@ExamId, @ExamineeId, @CreatedBy, @CreatedFrom, GETDATE());
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ExamId", examinee.ExamId);
                    cmd.Parameters.AddWithValue("@ExamineeId", examinee.ExamineeId);
                    cmd.Parameters.AddWithValue("@CreatedBy", examinee.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", examinee.CreatedFrom ?? (object)DBNull.Value);
                    examinee.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                result.Status = "Success";
                result.Message = "Exam inserted successfully.";
                result.Id = examinee.Id.ToString();
                result.DataVM = examinee;

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }

        public async Task<ResultVM> QuestionInsert(ExamQuestionHeaderVM question, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
                 INSERT INTO ExamQuestionHeaders
        (
            ExamId, ExamineeId, QuestionHeaderId,
            QuestionText, QuestionType, QuestionMark, MarkObtain
        )
        SELECT DISTINCT
            @ExamId, @ExamineeId, QH.Id,
            QH.QuestionText, QH.QuestionType, QH.QuestionMark, 0
        FROM QuestionHeaders QH
        INNER JOIN QuestionSetDetails QSD ON QSD.QuestionHeaderId = QH.Id
        WHERE QSD.QuestionSetHeaderId = @QuestionSetId
          AND NOT EXISTS (
              SELECT 1 
              FROM ExamQuestionHeaders EQH
              WHERE EQH.ExamId = @ExamId
                AND EQH.ExamineeId = @ExamineeId
                AND EQH.QuestionHeaderId = QH.Id
          );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ExamId", question.ExamId);
                    cmd.Parameters.AddWithValue("@ExamineeId", question.ExamineeId);
                    cmd.Parameters.AddWithValue("@QuestionSetId", question.QuestionSetId);

                    // ExecuteScalarAsync returns the first column of the first row
                    object resultObj = await cmd.ExecuteScalarAsync();

                    // SCOPE_IDENTITY() may return decimal, convert safely
                    question.Id = resultObj != DBNull.Value ? Convert.ToInt32(resultObj) : 0;
                }


                result.Status = "Success";
                result.Message = "Exam inserted successfully.";
                result.Id = question.Id.ToString();
                result.DataVM = question;

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }


        public async Task<ResultVM> InsertExamQuestionDetails(int examId, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                // 1. Get all ExamQuestionHeaders for this Exam
                string getHeadersQuery = @"
            SELECT Id, ExamineeId, QuestionHeaderId
            FROM ExamQuestionHeaders
            WHERE ExamId = @ExamId;
        ";

                var headers = new List<(int ExamQuestionHeaderId, int ExamineeId, int QuestionHeaderId)>();
                using (var cmd = new SqlCommand(getHeadersQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ExamId", examId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            headers.Add((
                                reader.GetInt32(0), // ExamQuestionHeaderId
                                reader.GetInt32(1), // ExamineeId
                                reader.GetInt32(2)  // QuestionHeaderId
                            ));
                        }
                    }
                }

                // 2. Insert ExamQuestionOptionDetails
                string insertOptionDetails = @"
            INSERT INTO ExamQuestionOptionDetails
            (ExamId, ExamQuestionHeaderId, QuestionHeaderId, QuestionOptionDetailId, QuestionOption, QuestionAnswer, ExamineeAnswer)
            SELECT @ExamId, @EQHId, QOD.QuestionHeaderId, QOD.Id, QOD.QuestionOption, QOD.QuestionAnswer, '-'
            FROM QuestionOptionDetails QOD
            WHERE QOD.QuestionHeaderId = @QuestionHeaderId
              AND NOT EXISTS (
                  SELECT 1 FROM ExamQuestionOptionDetails EOD
                  WHERE EOD.ExamId = @ExamId
                  AND EOD.ExamQuestionHeaderId = @EQHId
                  AND EOD.QuestionOptionDetailId = QOD.Id
              );
        ";

                foreach (var header in headers)
                {
                    using (var cmd = new SqlCommand(insertOptionDetails, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ExamId", examId);
                        cmd.Parameters.AddWithValue("@EQHId", header.ExamQuestionHeaderId);
                        cmd.Parameters.AddWithValue("@QuestionHeaderId", header.QuestionHeaderId);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // 3. Insert ExamQuestionShortDetails
                string insertShortDetails = @"
            INSERT INTO ExamQuestionShortDetails
            (ExamId, ExamQuestionHeaderId, QuestionHeaderId, QuestionShortDetailId, QuestionAnswer, ExamineeAnswer)
            SELECT @ExamId, @EQHId, QSDT.QuestionHeaderId, QSDT.Id, QSDT.QuestionAnswer, '-'
            FROM QuestionShortDetails QSDT
            WHERE QSDT.QuestionHeaderId = @QuestionHeaderId
              AND NOT EXISTS (
                  SELECT 1 FROM ExamQuestionShortDetails ESD
                  WHERE ESD.ExamId = @ExamId
                  AND ESD.ExamQuestionHeaderId = @EQHId
                  AND ESD.QuestionShortDetailId = QSDT.Id
              );
        ";

                foreach (var header in headers)
                {
                    using (var cmd = new SqlCommand(insertShortDetails, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ExamId", examId);
                        cmd.Parameters.AddWithValue("@EQHId", header.ExamQuestionHeaderId);
                        cmd.Parameters.AddWithValue("@QuestionHeaderId", header.QuestionHeaderId);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                result.Status = "Success";
                result.Message = "Exam question option and short-answer details inserted successfully.";

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }

        public async Task<ResultVM> RandomQuestionInsert(ExamQuestionHeaderVM question, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (conn == null) throw new Exception("Database connection failed!");
                if (transaction == null) transaction = conn.BeginTransaction();

                string query = @"
INSERT INTO ExamQuestionHeaders (
    ExamId, ExamineeId, QuestionHeaderId,
    QuestionText, QuestionType, QuestionMark, MarkObtain
)
SELECT TOP (@NoOfQuestion)
    @ExamId, 
    @ExamineeId, 
    QH.Id,
    QH.QuestionText,
    QH.QuestionType,
    QH.QuestionMark,
    0
FROM QuestionHeaders QH
WHERE QH.QuestionSubjectId = @QuestionSubjectId
  AND QH.QuestionType = @QuestionType
  AND NOT EXISTS (
      SELECT 1 FROM ExamQuestionHeaders EQH
      WHERE EQH.ExamId = @ExamId 
        AND EQH.ExamineeId = @ExamineeId
        AND EQH.QuestionHeaderId = QH.Id
  )
GROUP BY QH.Id, QH.QuestionText, QH.QuestionType, QH.QuestionMark
ORDER BY NEWID();
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ExamId", question.ExamId);
                    cmd.Parameters.AddWithValue("@ExamineeId", question.ExamineeId);
                    cmd.Parameters.AddWithValue("@QuestionSubjectId", question.QuestionSubjectId);
                    cmd.Parameters.AddWithValue("@QuestionType", question.QuestionType);
                    cmd.Parameters.AddWithValue("@NoOfQuestion", question.NumberOfQuestion);

                    object resultObj = await cmd.ExecuteScalarAsync();
                    question.Id = resultObj != DBNull.Value ? Convert.ToInt32(resultObj) : 0;
                }

                result.Status = "Success";
                result.Message = "Random questions inserted successfully.";
                result.Id = question.Id.ToString();
                result.DataVM = question;

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
        }

        public async Task<ResultVM> RandomSubjectGridDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                var data = new GridEntity<AutomatedExamDetailsVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT D.Id) AS totalcount
                from AutomatedExamDetails D
                     LEFT OUTER JOIN QuestionSubjects S ON D.SubjectId = S.Id
                     Where AutomatedExamId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<AutomatedExamDetailsVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,

                   ISNULL(D.Id,0) Id,
                     ISNULL(D.AutomatedExamId,0) AutomatedExamId,
                     ISNULL(D.SubjectId,0) SubjectId,
                     ISNULL(S.Name,0) SubjectName,
                     ISNULL(D.NumberOfQuestion,0) NumberOfQuestion,
                     ISNULL(D.QuestionType,'') QuestionType,
                     ISNULL(D.QuestionMark,0) QuestionMark

                     from AutomatedExamDetails D
                     LEFT OUTER JOIN QuestionSubjects S ON D.SubjectId = S.Id
                     Where AutomatedExamId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<AutomatedExamDetailsVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND rowindex <= (@skip + @take)
";
                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<AutomatedExamDetailsVM>.GetGridData_CMD(options, sqlQuery, "D.Id");

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
