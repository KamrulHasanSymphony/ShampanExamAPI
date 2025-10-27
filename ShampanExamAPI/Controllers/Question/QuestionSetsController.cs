using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Question;
using ShampanExam.Service.Common;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.Utility;
using System.Data;
using ShampanExamAPI.Configuration;
using static ShampanExamAPI.Configuration.HttpRequestHelper;

namespace ShampanExamAPI.Controllers.Question
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionSetsController : ControllerBase
    {
        QuestionSetService _questionSetService = new QuestionSetService();
        CommonService _commonService = new CommonService();

        // =========================== INSERT ===========================
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(QuestionSetHeaderVM questionSet)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.Insert(questionSet);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString(), DataVM = questionSet };
            }
        }

        // =========================== UPDATE ===========================
        [HttpPost("Update")]
        public async Task<ResultVM> Update(QuestionSetHeaderVM questionSet)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.Update(questionSet);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString(), DataVM = questionSet };
            }
        }

        // =========================== DELETE ===========================
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.MultipleDelete(vm);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString(), DataVM = vm };
            }
        }

        // =========================== LIST ===========================
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.List(new[] { "M.Id" }, new[] { vm.Id });
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString(), DataVM = vm };
            }
        }

        // =========================== LIST AS DATATABLE ===========================
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(QuestionSetHeaderVM vm)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.ListAsDataTable(new[] { "" }, new[] { "" });
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString(), DataVM = vm };
            }
        }

        // =========================== DROPDOWN ===========================
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.Dropdown();
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString() };
            }
        }

        // =========================== GRID DATA ===========================
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.GetGridData(
                    options,
                    new[] { "H.IsActive", "H.CreatedAt between", "H.CreatedAt between" },
                    new[] { "1", options.vm.FromDate?.ToString(), options.vm.ToDate?.ToString() }
                );
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString() };
            }
        }

        //// =========================== REPORT PREVIEW ===========================
        //[HttpPost("ReportPreview")]
        //public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        //{
        //    _commonService = new CommonService();
        //    ResultVM settingResult = new() { Status = "Fail", Message = "Error" };

        //    try
        //    {
        //        string baseUrl = "";

        //        // Fetch Report API URL from settings
        //        settingResult = await _commonService.SettingsValue(
        //            new[] { "SettingGroup", "SettingName" },
        //            new[] { "ExamReportUrl", "ExamReportUrl" },
        //            null
        //        );

        //        if (settingResult.Status == "Success" && settingResult.DataVM is DataTable settingTable && settingTable.Rows.Count > 0)
        //            baseUrl = settingTable.Rows[0]["SettingValue"].ToString();

        //        if (string.IsNullOrEmpty(baseUrl))
        //            throw new Exception("Report API URL not found!");

        //        // Create PeramModel for report
        //        PeramModel peramModel = new()
        //        {
        //            CompanyId = vm.CompanyId,
        //            BranchId = vm.BranchId
        //        };

        //        var resultVM = await _questionSetService.ReportPreview(new[] { "H.Id", "H.BranchId" }, new[] { vm.Id, vm.BranchId });

        //        if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
        //        {
        //            string json = ShampanExamAPI.Configuration.ExtensionMethods.DataTableToJson(dt);
        //            HttpRequestHelper http = new();

        //            var auth = http.GetAuthentication(new CredentialModel
        //            {
        //                ApiKey = DatabaseHelper.GetKey(),
        //                PathName = baseUrl
        //            });

        //            var stream = http.PostDataReport(baseUrl + "/api/QuestionSet/GetQuestionSetReport", auth, json);

        //            if (stream == null)
        //                throw new Exception("Failed to generate report.");

        //            return new FileStreamResult(stream, "application/pdf")
        //            {
        //                FileDownloadName = "QuestionSet.pdf"
        //            };
        //        }

        //        throw new Exception("No data found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error generating report: {ex.Message}");
        //    }
        //}

        // =========================== MULTIPLE POST ===========================
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.MultiplePost(vm);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString(), DataVM = vm };
            }
        }

        // =========================== GET DETAIL GRID BY HEADER ===========================
        [HttpPost("GetQuestionSetDetailDataById")]
        public async Task<ResultVM> GetQuestionSetDetailDataById(GridOptions options, int headerId)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.GetQuestionSetDetailDataById(options, headerId);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString() };
            }
        }

        // =========================== GRID DATA BY QUESTION ===========================
        [HttpPost("GetQuestionSetGridDataByQuestion")]
        public async Task<ResultVM> GetQuestionSetGridDataByQuestion(GridOptions options)
        {
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await _questionSetService.GetQuestionSetGridDataByQuestion(options, options.vm.Id);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.ToString() };
            }
        }


        [HttpPost("InsertQuestionSetQuestion")]
        public async Task<ResultVM> InsertQuestionSetQuestion(QuestionSetQuestionVM vm)
        {
            var service = new QuestionSetService();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                result = await service.InsertQuestionSetQuestion(vm);
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString(),
                    DataVM = vm
                };
            }
        }


        [HttpPost("QuestionSetQuestionList")]
        public async Task<ResultVM> QuestionSetQuestionList(CommonVM vm)
        {
            var service = new QuestionSetService();
            ResultVM result = new() { Status = "Fail", Message = "Error" };
            try
            {
                // example: filter by QuestionSetHeaderId if vm.Id is provided
                result = await service.QuestionSetQuestionList(
                    new[] { "QSQ.QuestionSetHeaderId" },
                    new[] { vm.Id },
                    null
                );
                return result;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString(),
                    DataVM = vm
                };
            }
        }

    }
}
