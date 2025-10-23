using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Common;
using ShampanExam.Service.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanTailor.Service.Question;
using ShampanTailor.ViewModel.QuestionVM;

namespace ShampanTailorAPI.Controllers.Question
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionHeaderController : ControllerBase
    {
        private readonly QuestionService _questionHeaderService;
        private readonly CommonService _common;

        public QuestionHeaderController()
        {
            _questionHeaderService = new QuestionService();
            _common = new CommonService();
        }

        // POST: api/QuestionHeader/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(QuestionHeaderVM questionHeader)
        {
            try
            {
                return await _questionHeaderService.Insert(questionHeader);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = questionHeader };
            }
        }

        // POST: api/QuestionHeader/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(QuestionHeaderVM questionHeader)
        {
            try
            {
                return await _questionHeaderService.Update(questionHeader);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = questionHeader };
            }
        }

        // POST: api/QuestionHeader/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            try
            {
                return await _questionHeaderService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = vm };
            }
        }

        // POST: api/QuestionHeader/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            try
            {
                return await _questionHeaderService.GetGridData(options);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message };
            }
        }


        // GET: api/QuestionHeader/Dropdown
        [HttpPost("Dropdown")]
        public async Task<ResultVM> Dropdown(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                resultVM = await _questionHeaderService.Dropdown(new[] { "" }, new[] { "" }, null);
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



    }
}
