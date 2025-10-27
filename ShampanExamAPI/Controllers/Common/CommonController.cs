using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Common;
using ShampanExam.Service.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.ExtensionMethods;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.SetUpVMs;
using System.Data;

namespace ShampanExamAPI.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {

        [HttpPost("NextPrevious")]
        public async Task<ResultVM> NextPrevious(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.NextPrevious(vm.Id, vm.Status, vm.TableName, vm.Type);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/Common/GetSettingsValue
        [HttpPost("GetSettingsValue")]
        public async Task<ResultVM> GetSettingsValue(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { vm.Group, vm.Name }, null);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable settingValue)
                {
                    resultVM.DataVM = null;
                    resultVM.DataVM = ExtensionMethods.ConvertDataTableToList(settingValue);
                }

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/Common/EnumList
        [HttpPost("EnumList")]
        public async Task<ResultVM> EnumList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.EnumList(new[] { "EnumType" }, new[] { Vm.Value.ToString() }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        //// POST: api/Common/CustomerList
        //[HttpPost("CustomerList")]
        //public async Task<ResultVM> CustomerList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CustomerList(new[] { "" }, new[] { "" }, null);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = "Data not fetched.",
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}

        // POST: api/Common/GetProductModalData
        [HttpPost("GetProductModalData")]
        public async Task<ResultVM> GetProductModalData(ProductVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();


                resultVM = await _commonService.GetProductModalData(new[] { "P.Code like", "P.Name like" }, new[] { model.Code, model.Name }, model.PeramModel);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }




        [HttpPost("QuestionDataForSet")]
        public async Task<ResultVM> QuestionDataForSet(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Value))
                {
                    conditionFields = new string[] { "Name" };
                    conditionValues = new string[] { Vm.Value };
                }

                QuestionSetHeaderService _questionSetHeaderService = new QuestionSetHeaderService();
                resultVM = await _questionSetHeaderService.QuestionList(conditionFields, conditionValues, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }


        //POST: api/Common/GetAllQuestionsByChapter
        [HttpPost("GetAllQuestionsByChapter")]
        public async Task<ResultVM> GetAllQuestionsByChapter([FromBody] questionRequest request)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CommonService _commonService = new CommonService();

            try
            {
                var options = request.Options;
                var chapterId = request.ChapterID;

                // Pass parameters to GetFromShadeGridData method
                resultVM = await _commonService.GetAllQuestionsByChapter(
                    options, new[] { "" }, new[] { "" }, chapterId
                );

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }


    }
}
