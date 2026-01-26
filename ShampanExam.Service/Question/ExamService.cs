using ShampanExam.Repository.Common;
using ShampanExam.Repository.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ShampanExam.Service.Question
{
    public class ExamService
    {
        CommonRepository _commonRepo = new CommonRepository();

        // Insert Method
        public async Task<ResultVM> Insert(ExamVM exam)
        {
            ExamRepository _repo = new ExamRepository();
            _commonRepo = new CommonRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                // Open SQL connection and transaction
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                // 1. Generate Exam Code
                string code = _commonRepo.GenerateCode("Exam", "Exam", exam.Date, exam.BranchId, conn, transaction);
                if (string.IsNullOrEmpty(code))
                    throw new Exception("Code Generation Failed!");

                exam.Code = code;

                // 2. Insert Exam
                result = await _repo.Insert(exam, conn, transaction);
                if (result.Status != "Success") throw new Exception(result.Message);

                int examId = Convert.ToInt32(result.Id);

                // 3. Insert Automated Exam Details and Random Questions
                if (exam.automatedExamDetailList != null && exam.automatedExamDetailList.Any())
                {
                    foreach (var item in exam.automatedExamDetailList)
                    {
                        item.Id = examId; // Link detail to exam

                        var subjectId = item.SubjectId;
                        var questionType = item.QuestionType;
                        var noOfQuestion = item.NumberOfQuestion;

                        // Insert automated exam detail
                        var detailResult = await _repo.DetailsInsert(item, conn, transaction);
                        if (detailResult.Status != "Success")
                            throw new Exception(detailResult.Message);

                        // Insert examinees and their questions for this detail
                        if (exam.examExamineeList != null && exam.examExamineeList.Any())
                        {
                            foreach (var examinee in exam.examExamineeList)
                            {
                                examinee.ExamId = examId.ToString();
                                examinee.CreatedBy = exam.CreatedBy;
                                examinee.CreatedFrom = exam.CreatedFrom;

                                // Insert examinee
                                var examineeResult = await _repo.ExamineeInsert(examinee, conn, transaction);
                                if (examineeResult.Status != "Success")
                                    throw new Exception(examineeResult.Message);

                                // Prepare question object for random insertion
                                var question = new ExamQuestionHeaderVM
                                {
                                    ExamId = examId,
                                    ExamineeId = examinee.ExamineeId,
                                    QuestionSetId = exam.QuestionSetId,
                                    QuestionSubjectId = subjectId,
                                    QuestionType = questionType,
                                    NumberOfQuestion = noOfQuestion
                                };

                                // Insert random questions for this examinee
                                var questionResult = await _repo.RandomQuestionInsert(question, conn, transaction);
                                if (questionResult.Status != "Success")
                                    throw new Exception(questionResult.Message);
                            }
                        }
                    }
                }
                else
                {
                    // If not automated, insert manually added questions per examinee
                    if (exam.examExamineeList != null && exam.examExamineeList.Any())
                    {
                        foreach (var examinee in exam.examExamineeList)
                        {
                            examinee.ExamId = examId.ToString();
                            examinee.CreatedBy = exam.CreatedBy;
                            examinee.CreatedFrom = exam.CreatedFrom;

                            var examineeResult = await _repo.ExamineeInsert(examinee, conn, transaction);
                            if (examineeResult.Status != "Success")
                                throw new Exception(examineeResult.Message);

                            // Insert each question for this examinee
                            if (exam.examQuestionHeaderList != null)
                            {
                                foreach (var question in exam.examQuestionHeaderList)
                                {
                                    question.ExamId = examId;
                                    question.ExamineeId = examinee.ExamineeId;
                                    question.QuestionSetId = exam.QuestionSetId;
                                    var questionResult = await _repo.QuestionInsert(question, conn, transaction);
                                    if (questionResult.Status != "Success")
                                        throw new Exception(questionResult.Message);
                                }
                            }
                        }
                    }
                }

                // 4. Insert Option and Short Answer Details
                var detailsResultFinal = await _repo.InsertExamQuestionDetails(examId, conn, transaction);
                if (detailsResultFinal.Status != "Success")
                    throw new Exception(detailsResultFinal.Message);

                transaction.Commit();

                result.Status = "Success";
                result.Message = "Exam inserted successfully.";
                result.Id = examId.ToString();
                result.DataVM = exam;
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                result.Id = exam.Code;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }

        // Insert Method
        public async Task<ResultVM> SelfInsert(ExamVM exam)
        {
            string CodeGroup = "Exam";
            string CodeName = "Exam";
            ExamRepository _repo = new ExamRepository();
            _commonRepo = new CommonRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            CommonVM commonVM = new CommonVM();

            commonVM.Group = "Exam";
            commonVM.Name = "Exam";

            _commonRepo = new CommonRepository();
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();



                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, exam.Date, exam.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    exam.Code = code;


                    result = await _repo.SelfInsert(exam, conn, transaction);
                    if (result.Status == "Success")
                    {
                        foreach (var item in exam.automatedExamDetailList)
                        {
                            item.Id = Convert.ToInt32(result.Id);
                            var resultt = await _repo.DetailsInsert(item, conn, transaction);
                            if (result.Status != "Success")
                            {
                                throw new Exception(resultt.Message);
                            }

                        }
                        if (isNewConnection && result.Status == "Success")
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            throw new Exception(result.Message);
                        }

                    }

                }
                else
                {
                    throw new Exception("Code Generation Failed!");
                }
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                result.Code = exam.Code;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }
        public async Task<ResultVM> Update(ExamVM exam)
        {
            ExamRepository _repo = new ExamRepository();
            _commonRepo = new CommonRepository();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                Id = exam.Id.ToString(),
                DataVM = exam
            };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                int examId = exam.Id;

                /* -------------------- 1. Update Exam -------------------- */
                result = await _repo.Update(exam, conn, transaction);
                if (result.Status != "Success")
                    throw new Exception(result.Message);

                /* -------------------- 2. Delete Existing Child Data -------------------- */
                _commonRepo.DetailsDelete("ExamExaminees", new[] { "ExamId" }, new[] { examId.ToString() }, conn, transaction);
                _commonRepo.DetailsDelete("ExamQuestionHeaders", new[] { "ExamId" }, new[] { examId.ToString() }, conn, transaction);
                _commonRepo.DetailsDelete("ExamQuestionOptionDetails", new[] { "ExamId" }, new[] { examId.ToString() }, conn, transaction);
                _commonRepo.DetailsDelete("ExamQuestionShortDetails", new[] { "ExamId" }, new[] { examId.ToString() }, conn, transaction);

                /* -------------------- 3. Automated Exam (Random Questions) -------------------- */
                if (exam.automatedExamDetailList != null && exam.automatedExamDetailList.Any())
                {
                    foreach (var item in exam.automatedExamDetailList)
                    {
                        item.Id = examId;

                        var subjectId = item.SubjectId;
                        var questionType = item.QuestionType;
                        var noOfQuestion = item.NumberOfQuestion;

                        // Insert automated exam detail
                        var detailResult = await _repo.DetailsInsert(item, conn, transaction);
                        if (detailResult.Status != "Success")
                            throw new Exception(detailResult.Message);

                        // Insert examinees and random questions
                        foreach (var examinee in exam.examExamineeList)
                        {
                            examinee.ExamId = examId.ToString();
                            examinee.CreatedBy = exam.CreatedBy;
                            examinee.CreatedFrom = exam.CreatedFrom;

                            var examineeResult = await _repo.ExamineeInsert(examinee, conn, transaction);
                            if (examineeResult.Status != "Success")
                                throw new Exception(examineeResult.Message);

                            var question = new ExamQuestionHeaderVM
                            {
                                ExamId = examId,
                                ExamineeId = examinee.ExamineeId,
                                QuestionSetId = exam.QuestionSetId,
                                QuestionSubjectId = subjectId,
                                QuestionType = questionType,
                                NumberOfQuestion = noOfQuestion
                            };

                            var questionResult = await _repo.RandomQuestionInsert(question, conn, transaction);
                            if (questionResult.Status != "Success")
                                throw new Exception(questionResult.Message);
                        }
                    }
                }
                else
                {
                    /* -------------------- 4. Manual Questions -------------------- */
                    foreach (var examinee in exam.examExamineeList)
                    {
                        examinee.ExamId = examId.ToString();
                        examinee.CreatedBy = exam.CreatedBy;
                        examinee.CreatedFrom = exam.CreatedFrom;

                        var examineeResult = await _repo.ExamineeInsert(examinee, conn, transaction);
                        if (examineeResult.Status != "Success")
                            throw new Exception(examineeResult.Message);

                        if (exam.examQuestionHeaderList != null)
                        {
                            foreach (var question in exam.examQuestionHeaderList)
                            {
                                question.ExamId = examId;
                                question.ExamineeId = examinee.ExamineeId;

                                var questionResult = await _repo.QuestionInsert(question, conn, transaction);
                                if (questionResult.Status != "Success")
                                    throw new Exception(questionResult.Message);
                            }
                        }
                    }
                }

                /* -------------------- 5. Insert Option & Short Answer Details -------------------- */
                var detailsResultFinal = await _repo.InsertExamQuestionDetails(examId, conn, transaction);
                if (detailsResultFinal.Status != "Success")
                    throw new Exception(detailsResultFinal.Message);

                transaction.Commit();

                result.Status = "Success";
                result.Message = "Exam updated successfully.";
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                    transaction.Rollback();

                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                    conn.Close();
            }
        }

        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", IDs = vm.IDs };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                result = await _repo.Delete(vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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

        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.List(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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


        public async Task<ResultVM> GetExamInfoReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetExamInfoReport(conditionalFields, conditionalValues, vm, conn, transaction);

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
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
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


        // GetProcessedData Method
        public async Task<ResultVM> GetProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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
        // GetProcessedData Method
        public async Task<ResultVM> ListOfProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM resuletlist = new ResultVM { Status = "Fail", Message = "Error" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                resuletlist = await _repo.ListOfProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && resuletlist.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(resuletlist.Message);

                return resuletlist;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                resuletlist.Message = ex.Message;
                resuletlist.ExMessage = ex.ToString();
                return resuletlist;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }

        //// ListAsDataTable Method
        //public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    ExamRepository _repo = new ExamRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
        //        conn.Open();
        //        isNewConnection = true;
        //        transaction = conn.BeginTransaction();

        //        result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, vm, conn, transaction);

        //        if (isNewConnection && result.Status == "Success")
        //            transaction.Commit();
        //        else
        //            throw new Exception(result.Message);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection) transaction.Rollback();
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null) conn.Close();
        //    }
        //}

        //// Dropdown Method
        //public async Task<ResultVM> Dropdown()
        //{
        //    ExamRepository _repo = new ExamRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
        //        conn.Open();
        //        isNewConnection = true;
        //        transaction = conn.BeginTransaction();

        //        result = await _repo.Dropdown(conn, transaction);

        //        if (isNewConnection && result.Status == "Success")
        //            transaction.Commit();
        //        else
        //            throw new Exception(result.Message);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection) transaction.Rollback();
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null) conn.Close();
        //    }
        //}

        // GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetGridData(options, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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


        // GetProcessedData Method
        public async Task<ResultVM> GetRandomProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetRandomProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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

        public async Task<ResultVM> GetUserRandomProcessedData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetUserRandomProcessedData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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

        // GetGridData Method
        public async Task<ResultVM> GetRandomGridData(GridOptions options)
        {
            ExamRepository _repo = new ExamRepository();
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

                result = await _repo.GetRandomGridData(options, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                    transaction.Commit();
                else
                    throw new Exception(result.Message);

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

        public async Task<ResultVM> RandomSubjectGridDataById(GridOptions options, int masterId)
        {
            ExamRepository _repo = new ExamRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.RandomSubjectGridDataById(options, masterId, conn, transaction);

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
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.Message.ToString();
                result.ExMessage = ex.ToString();
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
