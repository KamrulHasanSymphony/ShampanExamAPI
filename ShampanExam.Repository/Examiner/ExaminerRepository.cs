using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.Exam;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShampanExam.Repository.Exam
{
    public class ExaminerRepository : CommonRepository
    {
        //public async Task<ResultVM> QuestionList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction = null, PeramModel vm = null)
        //{
        //    var result = new ResultVM { Status = "Fail", Message = "Error" };

        //    try
        //    {
        //        // 1. Get all questionsd
        //        var questionQuery = @"
        //    SELECT Id, QuestionText, QuestionType, QuestionMark 
        //    FROM QuestionHeaders";

        //        var questions = new List<QuestionVM>();
        //        using (var cmd = new SqlCommand(questionQuery, conn, transaction))
        //        using (var reader = await cmd.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                questions.Add(new QuestionVM
        //                {
        //                    Id = reader.GetInt32(0),
        //                    QuestionText = reader.GetString(1),
        //                    QuestionType = reader.GetString(2),
        //                    QuestionMark = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
        //                });
        //            }
        //        }

        //        if (!questions.Any())
        //        {
        //            result.Status = "Success";
        //            result.Message = "No questions found.";
        //            result.DataVM = questions;
        //            return result;
        //        }

        //        // 2. Get options for radio/checkbox questions
        //        var optionQuestionIds = questions
        //            .Where(q => q.QuestionType.Equals("SingleOption", StringComparison.OrdinalIgnoreCase) ||
        //                        q.QuestionType.Equals("MultiOption", StringComparison.OrdinalIgnoreCase))
        //            .Select(q => q.Id)
        //            .ToList();

        //        if (optionQuestionIds.Any())
        //        {
        //            var optionQuery = $@"
        //        SELECT Id, QuestionHeaderId, QuestionOption, QuestionAnswer
        //        FROM QuestionOptionDetails
        //        WHERE QuestionHeaderId IN ({string.Join(",", optionQuestionIds)})";

        //            var options = new List<QuestionOptionVM>();
        //            using (var cmd = new SqlCommand(optionQuery, conn, transaction))
        //            using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    options.Add(new QuestionOptionVM
        //                    {
        //                        Id = reader.GetInt32(0),
        //                        QuestionHeaderId = reader.GetInt32(1),
        //                        QuestionOption = reader.GetString(2),
        //                        QuestionAnswer = (!reader.IsDBNull(3) && reader.GetBoolean(3)) ? "true" : "false"
        //                    });
        //                }
        //            }

        //            // Map options to questions
        //            foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
        //            {
        //                q.Options = options.Where(o => o.QuestionHeaderId == q.Id).ToList();
        //            }
        //        }

        //        // 3. Get answers for text questions
        //        var textQuestionIds = questions
        //            .Where(q => q.QuestionType.Equals("SingleLine", StringComparison.OrdinalIgnoreCase) ||
        //                        q.QuestionType.Equals("MultiLine", StringComparison.OrdinalIgnoreCase))
        //            .Select(q => q.Id)
        //            .ToList();

        //        if (textQuestionIds.Any())
        //        {
        //            var textQuery = $@"
        //        SELECT QuestionHeaderId, QuestionAnswer
        //        FROM QuestionShortDetails
        //        WHERE QuestionHeaderId IN ({string.Join(",", textQuestionIds)})";

        //            var textAnswers = new Dictionary<int, string>();
        //            using (var cmd = new SqlCommand(textQuery, conn, transaction))
        //            using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    int qId = reader.GetInt32(0);
        //                    string answer = reader.IsDBNull(1) ? null : reader.GetString(1);
        //                    textAnswers[qId] = answer;
        //                }
        //            }

        //            foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
        //            {
        //                if (textAnswers.TryGetValue(q.Id, out var ans))
        //                    q.CorrectAnswer = ans;
        //            }
        //        }

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = questions;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //    }

        //    return result;
        //}

        public async Task<ResultVM> QuestionList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction = null, PeramModel vm = null)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                var questionQuery = @"
    SELECT Id, ExamId, ExamineeId, QuestionText, QuestionType, QuestionMark,QuestionHeaderId,isnull(IsExamSubmitted,0)IsExamSubmitted
    FROM ExamQuestionHeaders m where 1=1 
";
                questionQuery = ApplyConditions(questionQuery, conditionalFields, conditionalValues, false);

                var questions = new List<QuestionVM>();

                // 2. Create command
                await using (var cmd = new SqlCommand(questionQuery, conn, transaction))
                {
                    // Apply parameters to the command (not adapter)
                    ApplyParameters(cmd, conditionalFields, conditionalValues);

                    // 3. Execute query and read results
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var question = new QuestionVM
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ExamId = reader.GetInt32(reader.GetOrdinal("ExamId")),
                                ExamineeId = reader.GetInt32(reader.GetOrdinal("ExamineeId")),
                                QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                                QuestionType = reader.GetString(reader.GetOrdinal("QuestionType")),
                                QuestionMark = reader.IsDBNull(reader.GetOrdinal("QuestionMark"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("QuestionMark")),
                                QuestionHeaderId = reader.GetInt32(reader.GetOrdinal("QuestionHeaderId")),
                                IsExamSubmitted = !reader.IsDBNull(7) && reader.GetBoolean(7),

                            };

                            questions.Add(question);
                        }
                    }
                }

                if (!questions.Any())
                {
                    result.Status = "Success";
                    result.Message = "No questions found.";
                    result.DataVM = questions;
                    return result;
                }

                // 2. Get options for radio/checkbox questions
                var optionQuestionIds = questions
                    .Where(q => q.QuestionType.Equals("SingleOption", StringComparison.OrdinalIgnoreCase) ||
                                q.QuestionType.Equals("MultiOption", StringComparison.OrdinalIgnoreCase))
                    .Select(q => q.Id)
                    .ToList();

                if (optionQuestionIds.Any())
                {
                    var optionQuery = $@"
                       SELECT Id, ExamId,ExamQuestionHeaderId, QuestionHeaderId, QuestionOption, QuestionAnswer,ExamineeAnswer
        FROM ExamQuestionOptionDetails 

                        WHERE ExamQuestionHeaderId IN ({string.Join(",", optionQuestionIds)})";

                    var options = new List<QuestionOptionVM>();
                    using (var cmd = new SqlCommand(optionQuery, conn, transaction))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            options.Add(new QuestionOptionVM
                            {
                                Id = reader.GetInt32(0),
                                ExamId = reader.GetInt32(1),
                                ExamQuestionHeaderId = reader.GetInt32(2),
                                QuestionHeaderId = reader.GetInt32(3),
                                QuestionOption = reader.GetString(4),
                                //QuestionAnswer = reader.GetString(5),
                                //QuestionAnswer = (!reader.IsDBNull(5) && reader.GetBoolean(5)) ? "true" : "false",
                                 ExamineeAnswer = !reader.IsDBNull(6) && reader.GetBoolean(6)
                            });
                        }
                    }

                    // Map options to questions
                    foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
                    {
                        q.Options = options.Where(o => o.ExamQuestionHeaderId == q.Id).ToList();


                    }
                    foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
                    {
                        q.Options = options
                            .Where(o => o.ExamQuestionHeaderId == q.Id && o.QuestionHeaderId == q.QuestionHeaderId)
                            .ToList();

                        // Fill SelectedOptionIds where ExamineeAnswer = true
                        q.SelectedOptionIds = q.Options
                            .Where(o => o.ExamineeAnswer)
                            .Select(o => o.Id ?? 0)
                            .ToList();
                    }

                }

                // 3. Get answers for text questions
                var textQuestionIds = questions
                    .Where(q => q.QuestionType.Equals("SingleLine", StringComparison.OrdinalIgnoreCase) ||
                                q.QuestionType.Equals("MultiLine", StringComparison.OrdinalIgnoreCase))
                    .Select(q => q.Id)
                    .ToList();

                if (textQuestionIds.Any())
                {
                    var textQuery = $@"
                        SELECT ExamId,ExamQuestionHeaderId,QuestionHeaderId, QuestionAnswer,ExamineeAnswer
                        FROM ExamQuestionShortDetails
                        WHERE ExamQuestionHeaderId IN ({string.Join(",", textQuestionIds)})";

                    var textAnswers = new Dictionary<int, string>();
                    var ExamineeAns= new Dictionary<int, string>();
                    using (var cmd = new SqlCommand(textQuery, conn, transaction))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int qId = reader.GetInt32(1);
                            string QuestionAnswer = reader.IsDBNull(3) ? null : reader.GetString(3);
                            string ExamineeAnswer = reader.IsDBNull(4) ? null : reader.GetString(4);
                            int ExamQuestionHeaderId = reader.GetInt32(1) ;
                            textAnswers[qId] = QuestionAnswer;
                            ExamineeAns[qId] = ExamineeAnswer;
                        }
                    }

                    //foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
                    //{
                    //    if (textAnswers.TryGetValue(q.Id, out var ans))
                    //        q.CorrectAnswer = ans;
                    //}
                    foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
                    {
                        if (ExamineeAns.TryGetValue(q.Id, out var ans))
                            q.UserAnswer = ans;
                    }
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = questions;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }

            return result;
        }



        public async Task<ResultVM> UpdateMarks(QuestionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = vm.Id.ToString(), DataVM = vm };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                UPDATE ExamQuestionHeaders 
                SET 
                    MarkObtain = @MarkObtain
                    
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                  
                    cmd.Parameters.AddWithValue("@MarkObtain", vm.ExaminerMarks);
                   

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Exam Question Marks updated successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows updated.");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }




        public async Task<ResultVM> ExamSubmit(QuestionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = vm.Id.ToString(), DataVM = vm };
            try
            {
                if (conn == null) throw new Exception("Database connection failed!");

                string query = @"
                UPDATE ExamQuestionHeaders 
                SET 
                    IsExamMarksSubmitted = @IsExamMarksSubmitted
                    
                WHERE ExamineeId = @ExamineeId and ExamId=@ExamId";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ExamineeId", vm.ExamineeId);
                    cmd.Parameters.AddWithValue("@ExamId", vm.ExamId);
                    cmd.Parameters.AddWithValue("@IsExamMarksSubmitted", true);


                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Exam Question Marks Submitted successfully.";
                    }
                    else
                    {
                        result.Status = "Success";
                        result.Message = "Exam Question Marks Submitted successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }

        public async Task<ResultVM> QuestionAnsList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction = null, PeramModel vm = null)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                var questionQuery = @"
    SELECT Id, ExamId, ExamineeId, QuestionText, QuestionType, QuestionMark,QuestionHeaderId,isnull(IsExamMarksSubmitted,0)IsExamSubmitted,MarkObtain
    FROM ExamQuestionHeaders m where 1=1 
";
                questionQuery = ApplyConditions(questionQuery, conditionalFields, conditionalValues, false);

                var questions = new List<QuestionVM>();

                // 2. Create command
                await using (var cmd = new SqlCommand(questionQuery, conn, transaction))
                {
                    // Apply parameters to the command (not adapter)
                    ApplyParameters(cmd, conditionalFields, conditionalValues);

                    // 3. Execute query and read results
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var question = new QuestionVM
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ExamId = reader.GetInt32(reader.GetOrdinal("ExamId")),
                                ExamineeId = reader.GetInt32(reader.GetOrdinal("ExamineeId")),
                                QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                                QuestionType = reader.GetString(reader.GetOrdinal("QuestionType")),
                                QuestionMark = reader.IsDBNull(reader.GetOrdinal("QuestionMark"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("QuestionMark")),
                                QuestionHeaderId = reader.GetInt32(reader.GetOrdinal("QuestionHeaderId")),
                                IsExamSubmitted = !reader.IsDBNull(7) && reader.GetBoolean(7),
                                ExaminerMarks = reader.GetDecimal(reader.GetOrdinal("MarkObtain")),


                            };

                            questions.Add(question);
                        }
                    }
                }

                if (!questions.Any())
                {
                    result.Status = "Success";
                    result.Message = "No questions found.";
                    result.DataVM = questions;
                    return result;
                }

                // 2. Get options for radio/checkbox questions
                var optionQuestionIds = questions
                    .Where(q => q.QuestionType.Equals("SingleOption", StringComparison.OrdinalIgnoreCase) ||
                                q.QuestionType.Equals("MultiOption", StringComparison.OrdinalIgnoreCase))
                    .Select(q => q.Id)
                    .ToList();

                if (optionQuestionIds.Any())
                {
                    var optionQuery = $@"
                       SELECT Id, ExamId,ExamQuestionHeaderId, QuestionHeaderId, QuestionOption, QuestionAnswer,ExamineeAnswer
        FROM ExamQuestionOptionDetails 

                        WHERE ExamQuestionHeaderId IN ({string.Join(",", optionQuestionIds)})";

                    var options = new List<QuestionOptionVM>();
                    using (var cmd = new SqlCommand(optionQuery, conn, transaction))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            options.Add(new QuestionOptionVM
                            {
                                Id = reader.GetInt32(0),
                                ExamId = reader.GetInt32(1),
                                ExamQuestionHeaderId = reader.GetInt32(2),
                                QuestionHeaderId = reader.GetInt32(3),
                                QuestionOption = reader.GetString(4),
                                //QuestionAnswer = reader.GetString(5),
                                QuestionAnswer = (!reader.IsDBNull(5) && reader.GetBoolean(5)) ? "true" : "false",
                                ExamineeAnswer = !reader.IsDBNull(6) && reader.GetBoolean(6)
                            });
                        }
                    }

                    // Map options to questions
                    foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
                    {
                        q.Options = options.Where(o => o.ExamQuestionHeaderId == q.Id).ToList();


                    }
                    foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
                    {
                        q.Options = options
                            .Where(o => o.ExamQuestionHeaderId == q.Id && o.QuestionHeaderId == q.QuestionHeaderId)
                            .ToList();

                        // Fill SelectedOptionIds where ExamineeAnswer = true
                        q.SelectedOptionIds = q.Options
                            .Where(o => o.ExamineeAnswer)
                            .Select(o => o.Id ?? 0)
                            .ToList();
                    }

                }

                // 3. Get answers for text questions
                var textQuestionIds = questions
                    .Where(q => q.QuestionType.Equals("SingleLine", StringComparison.OrdinalIgnoreCase) ||
                                q.QuestionType.Equals("MultiLine", StringComparison.OrdinalIgnoreCase))
                    .Select(q => q.Id)
                    .ToList();

                if (textQuestionIds.Any())
                {
                    var textQuery = $@"
                        SELECT ExamId,ExamQuestionHeaderId,QuestionHeaderId, QuestionAnswer,ExamineeAnswer
                        FROM ExamQuestionShortDetails
                        WHERE ExamQuestionHeaderId IN ({string.Join(",", textQuestionIds)})";

                    var textAnswers = new Dictionary<int, string>();
                    var ExamineeAns = new Dictionary<int, string>();
                    using (var cmd = new SqlCommand(textQuery, conn, transaction))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int qId = reader.GetInt32(1);
                            string QuestionAnswer = reader.IsDBNull(3) ? null : reader.GetString(3);
                            string ExamineeAnswer = reader.IsDBNull(4) ? null : reader.GetString(4);
                            int ExamQuestionHeaderId = reader.GetInt32(1);
                            textAnswers[qId] = QuestionAnswer;
                            ExamineeAns[qId] = ExamineeAnswer;
                        }
                    }

                    foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
                    {
                        if (textAnswers.TryGetValue(q.Id, out var ans))
                            q.CorrectAnswer = ans;
                    }
                    foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
                    {
                        if (ExamineeAns.TryGetValue(q.Id, out var ans))
                            q.UserAnswer = ans;
                    }
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = questions;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }

            return result;
        }


    }
}





