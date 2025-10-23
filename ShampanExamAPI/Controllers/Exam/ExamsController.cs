using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.Common;
using ShampanExam.Service.Exam;
using ShampanExam.ViewModel.CommonVMs;

namespace ShampanExamAPI.Controllers.Exam
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        ExamService _examService = new ExamService();
        CommonService _commonService = new CommonService();



        // POST: api/Exams/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                result = await _examService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
                return result;
            }
            catch (System.Exception ex)
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
    }
}
