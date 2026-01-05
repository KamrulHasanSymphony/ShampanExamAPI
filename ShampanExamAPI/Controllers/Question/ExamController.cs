using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Common;
using ShampanExam.Service.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.ExtensionMethods;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using ShampanExamAPI.Configuration;
using System.Data;
using static ShampanExamAPI.Configuration.HttpRequestHelper;

namespace ShampanExamAPI.Controllers.Question
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        ExamService _examService = new ExamService();
        CommonService _common = new CommonService();

        // POST: api/Exam/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ExamVM exam)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExamService();
                resultVM = await _examService.Insert(exam);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = exam };
            }
        }

        // POST: api/Exam/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ExamVM exam)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExamService();
                resultVM = await _examService.Update(exam);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = exam };
            }
        }

        // POST: api/Exam/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExamService();
                resultVM = await _examService.Delete(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = vm };
            }
        }

        // POST: api/Exam/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExamService();
                resultVM = await _examService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = vm };
            }
        }


        [HttpPost("ExamInfoReport")]
        public async Task<ResultVM> ExamInfoReport(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _examService = new ExamService();
                resultVM = await _examService.GetExamInfoReport(new[] { "EX.Id" }, new[] { vm.Id }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }




        // POST: api/Exam/GetProcessedData
        [HttpPost("GetProcessedData")]
        public async Task<IActionResult> GetProcessedData([FromBody] CommonVM vm)
        {
            try
            {
                _examService = new ExamService();

                var result = await _examService.GetProcessedData(new[] { "Id", "ExamineeGroupId", "QuestionSetId" }, new[] { vm.Id, vm.Group, vm.Value }, null);

                if (result.Status == "Success")
                {
                    var resultList = await _examService.ListOfProcessedData(new[] { "Id" }, new[] { vm.Id }, null);

                    return Ok(resultList);
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString(),
                    DataVM = vm
                });
            }
        }


        //// GET: api/Exam/ListAsDataTable
        //[HttpGet("ListAsDataTable")]
        //public async Task<ResultVM> ListAsDataTable(ExamVM exam)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
        //    try
        //    {
        //        _examService = new ExamService();
        //        resultVM = await _examService.ListAsDataTable(new[] { "" }, new[] { "" });
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = exam };
        //    }
        //}

        //// GET: api/Exam/Dropdown
        //[HttpGet("Dropdown")]
        //public async Task<ResultVM> Dropdown()
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
        //    try
        //    {
        //        _examService = new ExamService();
        //        resultVM = await _examService.Dropdown();
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message };
        //    }
        //}

        // POST: api/Exam/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExamService();
                resultVM = await _examService.GetGridData(options);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message };
            }
        }

        //// POST: api/Exam/ReportPreview
        //[HttpPost("ReportPreview")]
        //public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        //{
        //    _common = new CommonService();
        //    ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error" };

        //    try
        //    {
        //        string baseUrl = "";

        //        settingResult = await _common.SettingsValue(new[] { "SettingGroup", "SettingName" },
        //                                                   new[] { "DMSReportUrl", "DMSReportUrl" }, null);

        //        if (settingResult.Status == "Success" && settingResult.DataVM is DataTable settingValue)
        //        {
        //            if (settingValue.Rows.Count > 0)
        //            {
        //                baseUrl = settingValue.Rows[0]["SettingValue"].ToString();
        //            }
        //        }

        //        if (string.IsNullOrEmpty(baseUrl))
        //            throw new Exception("Report API Url Not Found!");

        //        _examService = new ExamService();
        //        PeramModel peramModel = new PeramModel { CompanyId = vm.CompanyId };

        //        var resultVM = await _examService.ReportPreview(
        //            new[] { "H.Id", "H.BranchId" }, new[] { vm.Id, vm.BranchId }, peramModel);

        //        if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
        //        {
        //            string json = ExtensionMethods.DataTableToJson(dt);
        //            HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

        //            var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
        //            {
        //                ApiKey = DatabaseHelper.GetKey(),
        //                PathName = baseUrl
        //            });

        //            var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Exam/GetExam", authModel, json);

        //            if (stream == null) throw new Exception("Failed to generate report.");

        //            return new FileStreamResult(stream, "application/pdf")
        //            {
        //                FileDownloadName = "ExamReport.pdf"
        //            };
        //        }

        //        throw new Exception("No data found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error generating report: {ex.Message}");
        //    }
        //}

        // POST: api/Exam/GetRandomProcessedData
        [HttpPost("GetRandomProcessedData")]
        public async Task<IActionResult> GetRandomProcessedData([FromBody] CommonVM vm)
        {
            try
            {
                _examService = new ExamService();

                var result = await _examService.GetRandomProcessedData(new[] { "Id", "ExamineeGroupId", "QuestionSubjectId", "QuestionType", "NoOfQuestion" }, new[] { vm.Id, vm.Group, vm.QuestionSubjectId, vm.QuestionType, vm.NoOfQuestion }, null);

                if (result.Status == "Success")
                {
                    var resultList = await _examService.ListOfProcessedData(new[] { "Id" }, new[] { vm.Id }, null);

                    return Ok(resultList);
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString(),
                    DataVM = vm
                });
            }
        }

        // POST: api/Exam/GetUserRandomProcessedData
        [HttpPost("GetUserRandomProcessedData")]
        public async Task<IActionResult> GetUserRandomProcessedData([FromBody] CommonVM vm)
        {
            try
            {
                _examService = new ExamService();

                var result = await _examService.GetUserRandomProcessedData(new[] { "ExamId","QuestionSubjectId", "QuestionType", "NoOfQuestion" }, new[] { vm.Id,vm.QuestionSubjectId, vm.QuestionType, vm.NoOfQuestion }, null);

                if (result.Status == "Success")
                {
                    var resultList = await _examService.ListOfProcessedData(new[] { "Id" }, new[] { vm.Id }, null);

                    return Ok(resultList);
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString(),
                    DataVM = vm
                });
            }
        }
    }
}
