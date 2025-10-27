using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.Exam;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.Repository.Exam
{
    public class ExamRepository : CommonRepository
    {
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction = null, PeramModel vm = null)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                // 1. Get all questions
                var questionQuery = @"
            SELECT Id, QuestionText, QuestionType, QuestionMark 
            FROM QuestionHeaders";

                var questions = new List<QuestionVM>();
                using (var cmd = new SqlCommand(questionQuery, conn, transaction))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        questions.Add(new QuestionVM
                        {
                            Id = reader.GetInt32(0),
                            QuestionText = reader.GetString(1),
                            QuestionType = reader.GetString(2),
                            QuestionMark = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                        });
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
                SELECT Id, QuestionHeaderId, QuestionOption, QuestionAnswer
                FROM QuestionOptionquestionSetDetailList
                WHERE QuestionHeaderId IN ({string.Join(",", optionQuestionIds)})";

                    var options = new List<QuestionOptionVM>();
                    using (var cmd = new SqlCommand(optionQuery, conn, transaction))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            options.Add(new QuestionOptionVM
                            {
                                Id = reader.GetInt32(0),
                                QuestionHeaderId = reader.GetInt32(1),
                                QuestionOption = reader.GetString(2),
                                QuestionAnswer = (!reader.IsDBNull(3) && reader.GetBoolean(3)) ? "true" : "false"
                            });
                        }
                    }

                    // Map options to questions
                    foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
                    {
                        q.Options = options.Where(o => o.QuestionHeaderId == q.Id).ToList();
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
                SELECT QuestionHeaderId, QuestionAnswer
                FROM QuestionShortquestionSetDetailList
                WHERE QuestionHeaderId IN ({string.Join(",", textQuestionIds)})";

                    var textAnswers = new Dictionary<int, string>();
                    using (var cmd = new SqlCommand(textQuery, conn, transaction))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int qId = reader.GetInt32(0);
                            string answer = reader.IsDBNull(1) ? null : reader.GetString(1);
                            textAnswers[qId] = answer;
                        }
                    }

                    foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
                    {
                        if (textAnswers.TryGetValue(q.Id, out var ans))
                            q.CorrectAnswer = ans;
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




//using ShampanExam.Repository.Common;
//using ShampanExam.ViewModel.CommonVMs;
//using ShampanExam.ViewModel.Exam;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ShampanExam.Repository.Exam
//{
//    public class ExamRepository : CommonRepository
//    {
//        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction = null, PeramModel vm = null)
//        {
//            var result = new ResultVM { Status = "Fail", Message = "Error" };

//            try
//            {
//                // 1. Get all questions
//                var questionQuery = @"
//            SELECT Id, ExamId,ExamineeId,QuestionText, QuestionType, QuestionMark 
//FROM ExamQuestionHeaders";

//                var questions = new List<QuestionVM>();
//                using (var cmd = new SqlCommand(questionQuery, conn, transaction))
//                using (var reader = await cmd.ExecuteReaderAsync())
//                {
//                    while (await reader.ReadAsync())
//                    {
//                        questions.Add(new QuestionVM
//                        {
//                            Id = reader.GetInt32(0),
//                            QuestionText = reader.GetString(1),
//                            QuestionType = reader.GetString(2),
//                            QuestionMark = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
//                        });
//                    }
//                }

//                if (!questions.Any())
//                {
//                    result.Status = "Success";
//                    result.Message = "No questions found.";
//                    result.DataVM = questions;
//                    return result;
//                }

//                // 2. Get options for radio/checkbox questions
//                var optionQuestionIds = questions
//                    .Where(q => q.QuestionType.Equals("SingleOption", StringComparison.OrdinalIgnoreCase) ||
//                                q.QuestionType.Equals("MultiOption", StringComparison.OrdinalIgnoreCase))
//                    .Select(q => q.Id)
//                    .ToList();

//                if (optionQuestionIds.Any())
//                {
//                    var optionQuery = $@"
//               SELECT Id, ExamId,ExamQuestionHeaderId, QuestionHeaderId, QuestionOption, QuestionAnswer
//FROM ExamQuestionOptionquestionSetDetailList

//                WHERE ExamQuestionOptionquestionSetDetailList IN ({string.Join(",", optionQuestionIds)})";

//                    var options = new List<QuestionOptionVM>();
//                    using (var cmd = new SqlCommand(optionQuery, conn, transaction))
//                    using (var reader = await cmd.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            options.Add(new QuestionOptionVM
//                            {
//                                Id = reader.GetInt32(0),
//                                QuestionHeaderId = reader.GetInt32(1),
//                                QuestionOption = reader.GetString(2),
//                                QuestionAnswer = (!reader.IsDBNull(3) && reader.GetBoolean(3)) ? "true" : "false"
//                            });
//                        }
//                    }

//                    // Map options to questions
//                    foreach (var q in questions.Where(q => optionQuestionIds.Contains(q.Id)))
//                    {
//                        q.Options = options.Where(o => o.QuestionHeaderId == q.Id).ToList();
//                    }
//                }

//                // 3. Get answers for text questions
//                var textQuestionIds = questions
//                    .Where(q => q.QuestionType.Equals("SingleLine", StringComparison.OrdinalIgnoreCase) ||
//                                q.QuestionType.Equals("MultiLine", StringComparison.OrdinalIgnoreCase))
//                    .Select(q => q.Id)
//                    .ToList();

//                if (textQuestionIds.Any())
//                {
//                    var textQuery = $@"
//                SELECT ExamId,ExamQuestionHeaderId,QuestionHeaderId, QuestionAnswer
//                FROM ExamQuestionShortquestionSetDetailList
//                WHERE QuestionHeaderId IN ({string.Join(",", textQuestionIds)})";

//                    var textAnswers = new Dictionary<int, string>();
//                    using (var cmd = new SqlCommand(textQuery, conn, transaction))
//                    using (var reader = await cmd.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            int qId = reader.GetInt32(0);
//                            string answer = reader.IsDBNull(1) ? null : reader.GetString(1);
//                            textAnswers[qId] = answer;
//                        }
//                    }

//                    foreach (var q in questions.Where(q => textQuestionIds.Contains(q.Id)))
//                    {
//                        if (textAnswers.TryGetValue(q.Id, out var ans))
//                            q.CorrectAnswer = ans;
//                    }
//                }

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = questions;
//            }
//            catch (Exception ex)
//            {
//                result.Message = ex.Message;
//                result.ExMessage = ex.ToString();
//            }

//            return result;
//        }

//    }
//}
