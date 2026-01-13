using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShampanExam.Service.Common;
using ShampanExam.Service.Exam;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.Exam;
using ShampanExam.ViewModel.QuestionVM;

namespace ShampanExamAPI.Controllers.Exam
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        //ExamService _examService = new ExamService();
        CommonService _commonService = new CommonService();

        ExamService _examService = new ExamService();

        // POST: api/Exams/List
        [HttpPost("QuestionList")]
        public async Task<ResultVM> QuestionList(CommonVM vm)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                result = await _examService.QuestionList(new[] { "M.ExamineeId" }, new[] { vm.Id }, null);
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


        // POST: api/Exam/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(List<QuestionVM> Answers)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                _examService = new ExamService();
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
                _examService = new ExamService();
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
                result = await _examService.QuestionAnsList(new[] { "M.ExamineeId", "M.ExamId" }, new[] { vm.Id, vm.ExamId }, null);
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

        [HttpPost("QuestionAnsListSelf")]
        public async Task<ResultVM> QuestionAnsListSelf(CommonVM vm)
        {
            var result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                result = await _examService.QuestionAnsList(new[] { "M.ExamineeId", "M.ExamId" }, new[] { vm.Id,vm.ExamId }, null);
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
