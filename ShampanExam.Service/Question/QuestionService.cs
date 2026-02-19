using ShampanExam.Repository.Common;
using ShampanExam.Repository.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.Utility;
using ShampanTailor.Repository.Question;
using ShampanTailor.ViewModel.QuestionVM;
using System.Data.SqlClient;

namespace ShampanTailor.Service.Question
{
    public class QuestionService
    {
        CommonRepository _commonRepo = new CommonRepository();
        // Insert
        public async Task<ResultVM> Insert(QuestionHeaderVM questionHeader)
        {
            QuestionHeaderRepository _repo = new QuestionHeaderRepository();
            QuestionOptionDetailRepository optionquestionSetDetailListRepository = new QuestionOptionDetailRepository();
            QuestionShortDetailRepository shortquestionSetDetailListRepository = new QuestionShortDetailRepository();
            _commonRepo = new CommonRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };
            string CodeGroup = "Question";
            string CodeName = "Question";
            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();
                if (questionHeader.QuestionType== "MultiOption"|| questionHeader.QuestionType == "SingleOption")
                {
                    if (questionHeader.QuestionOptionDetails == null || !questionHeader.QuestionOptionDetails.Any())
                    {
                        throw new Exception("Question Option questionSetDetailList must have at least one detail!");
                    }
                }
                if (questionHeader.QuestionType == "MultiLine" || questionHeader.QuestionType == "SingleLine")
                {
                    if (questionHeader.QuestionShortDetails == null || !questionHeader.QuestionShortDetails.Any())
                    {
                        throw new Exception("Question Short questionSetDetailList must have at least one detail!");
                    }
                }
                #region Check Exist Data
                string[] conditionField = { "QuestionText" };
                string[] conditionValue = { questionHeader.QuestionText.Trim() };

                bool exist = _commonRepo.CheckExists("QuestionHeaders", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion
                string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);


                if (!string.IsNullOrEmpty(code))
                {
                    questionHeader.Code = code;

                    result = await _repo.Insert(questionHeader, conn, transaction);
                    questionHeader.Id = Convert.ToInt32(result.Id);

                if (result.Status.ToLower() == "success")
                {
                    // Insert Question Option questionSetDetailList
                    int LineNo = 1;
                    foreach (var optionDetail in questionHeader.QuestionOptionDetails)
                    {
                        optionDetail.QuestionHeaderId = questionHeader.Id;

                            var resultOption = await optionquestionSetDetailListRepository.Insert(optionDetail, conn, transaction);
                            if (resultOption.Status.ToLower() != "success")
                            {
                                throw new Exception(resultOption.Message);
                            }

                            LineNo++;
                        }

                    // Insert Question Short questionSetDetailList
                    foreach (var shortDetail in questionHeader.QuestionShortDetails)
                    {
                        shortDetail.QuestionHeaderId = questionHeader.Id;

                            var resultShortDetail = await shortquestionSetDetailListRepository.Insert(shortDetail, conn, transaction);
                            if (resultShortDetail.Status.ToLower() != "success")
                            {
                                throw new Exception(resultShortDetail.Message);
                            }
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

                    return result;
                }
                else
                {
                    throw new Exception("Code Generation Failed!");
                }
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

        // Update
        public async Task<ResultVM> Update(QuestionHeaderVM questionHeader)
        {
            QuestionHeaderRepository _repo = new QuestionHeaderRepository();
            QuestionOptionDetailRepository optionquestionSetDetailListRepository = new QuestionOptionDetailRepository();
            QuestionShortDetailRepository shortquestionSetDetailListRepository = new QuestionShortDetailRepository();
            _commonRepo = new CommonRepository();
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

                if ((questionHeader.QuestionOptionDetails == null || !questionHeader.QuestionOptionDetails.Any())
                 && (questionHeader.QuestionShortDetails == null || !questionHeader.QuestionShortDetails.Any()))
                {
                    throw new Exception("Question must have at least one detail (Option or Short)!");
                }

                // Delete existing questionSetDetailList
                var optionDeleteResult = _commonRepo.DetailsDelete("QuestionOptionDetails", new[] { "QuestionHeaderId" }, new[] { questionHeader.Id.ToString() }, conn, transaction);
                if (optionDeleteResult.Status == "Fail") throw new Exception("Error in Delete for Question Option questionSetDetailList.");

                var shortDeleteResult = _commonRepo.DetailsDelete("QuestionShortDetails", new[] { "QuestionHeaderId" }, new[] { questionHeader.Id.ToString() }, conn, transaction);
                if (shortDeleteResult.Status == "Fail") throw new Exception("Error in Delete for Question Short questionSetDetailList.");

                result = await _repo.Update(questionHeader, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    // Insert new Question Option questionSetDetailList
                    int LineNo = 1;
                    foreach (var optionDetail in questionHeader.QuestionOptionDetails)
                    {
                        optionDetail.QuestionHeaderId = questionHeader.Id;

                        var resultOption = await optionquestionSetDetailListRepository.Insert(optionDetail, conn, transaction);
                        if (resultOption.Status.ToLower() != "success")
                        {
                            throw new Exception(resultOption.Message);
                        }

                        LineNo++;
                    }

                    // Insert new Question Short questionSetDetailList
                    foreach (var shortDetail in questionHeader.QuestionShortDetails)
                    {
                        shortDetail.QuestionHeaderId = questionHeader.Id;

                        var resultShortDetail = await shortquestionSetDetailListRepository.Insert(shortDetail, conn, transaction);
                        if (resultShortDetail.Status.ToLower() != "success")
                        {
                            throw new Exception(resultShortDetail.Message);
                        }
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

        // List
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            QuestionHeaderRepository _repo = new QuestionHeaderRepository();
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

        // GetGridData
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            QuestionHeaderRepository _repo = new QuestionHeaderRepository();
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

        // Dropdown Method
        public async Task<ResultVM> Dropdown(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            QuestionHeaderRepository _repo = new QuestionHeaderRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionStringQuestion());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.Dropdown(conditionalFields, conditionalValues, vm, conn, transaction);

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
                result.Message = ex.ToString();
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
