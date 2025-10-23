using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Common;
using ShampanExam.Service.Grades;
using ShampanExam.Service.Question;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;

namespace ShampanTailorAPI.Controllers.Grades
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly GradeHeaderService _gradeHeaderService;
        private readonly CommonService _common;

        public GradeController()
        {
            _gradeHeaderService = new GradeHeaderService();
            _common = new CommonService();
        }

        // POST: api/Grade/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(GradeHeaderVM gradeHeader)
        {
            try
            {
                return await _gradeHeaderService.Insert(gradeHeader);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = gradeHeader };
            }
        }

        // POST: api/Grade/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(GradeHeaderVM gradeHeader)
        {
            try
            {
                return await _gradeHeaderService.Update(gradeHeader);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = gradeHeader };
            }
        }

        // POST: api/Grade/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            try
            {
                return await _gradeHeaderService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = vm };
            }
        }

        // POST: api/Grade/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            try
            {
                return await _gradeHeaderService.GetGridData(options);
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message };
            }
        }

        // GET: api/Grade/Dropdown
        [HttpPost("Dropdown")]
        public async Task<ResultVM> Dropdown(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                resultVM = await _gradeHeaderService.Dropdown(new[] { "" }, new[] { "" }, null);
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
