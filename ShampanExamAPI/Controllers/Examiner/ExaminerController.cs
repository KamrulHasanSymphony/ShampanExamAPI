using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShampanExam.Service.Common;
using ShampanExam.Service.Exam;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.Exam;
using ShampanExam.ViewModel.QuestionVM;

namespace ShampanExamAPI.Controllers.Examiner
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExaminerController : ControllerBase
    {
        CommonService _commonService = new CommonService();

        ExaminerService _examService = new ExaminerService();




        // POST: api/Exam/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(List<QuestionVM> Answers)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExaminerService();
                resultVM = await _examService.Insert(Answers);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = Answers };
            }
        }


        [HttpPost("ExamSubmit")]
        public async Task<ResultVM> ExamSubmit(QuestionVM Answers)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExaminerService();
                resultVM = await _examService.ExamSubmit(Answers);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = ex.Message, ExMessage = ex.Message, DataVM = Answers };
            }
        }

        [HttpPost("QuestionAnsList")]
        public async Task<ResultVM> QuestionAnsList(CommonVM vm)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                result = await _examService.QuestionAnsList(new[] { "M.ExamineeId","M.ExamId" }, new[] { vm.Id,vm.ExamId }, null);
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
