using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Common;
using ShampanExam.Service.Exam;
using ShampanExam.Service.Grades;
using ShampanExam.Service.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;

namespace ShampanExamAPI.Controllers.Question
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionSetController : ControllerBase
    {
        private readonly QuestionSetHeaderService _questionSetHeaderService;
        private readonly CommonService _common;

        public QuestionSetController()
        {
            _questionSetHeaderService = new QuestionSetHeaderService();
            _common = new CommonService();
        }

        // POST: api/QuestionSet/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(QuestionSetHeaderVM questionSetHeader)
        {
            try
            {
                return await _questionSetHeaderService.Insert(questionSetHeader);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = questionSetHeader };
            }
        }

        // POST: api/QuestionSet/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(QuestionSetHeaderVM questionSetHeader)
        {
            try
            {
                return await _questionSetHeaderService.Update(questionSetHeader);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = questionSetHeader };
            }
        }

        // POST: api/QuestionSet/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            try
            {
                return await _questionSetHeaderService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = vm };
            }
        }

        // POST: api/QuestionSet/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            try
            {
                return await _questionSetHeaderService.GetGridData(options);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message };
            }
        }


        // GET: api/QuestionSet/Dropdown
        [HttpPost("Dropdown")]
        public async Task<ResultVM> Dropdown(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                resultVM = await _questionSetHeaderService.Dropdown(new[] { "" }, new[] { "" }, null);
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

        //POST: api/QuestionSet/GetQuestionGridData
        [HttpPost("GetQuestionGridData")]
        public async Task<ResultVM> GetQuestionGridData([FromBody] examineeRequest request)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                var options = request.Options;
                var groupId = request.GroupId;
                // Pass parameters to GetFromShadeGridData method
                resultVM = await _questionSetHeaderService.GetQuestionGridData(
                    options, new[] { "" }, new[] { "" }, groupId
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
