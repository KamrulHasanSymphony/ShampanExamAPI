using Microsoft.AspNetCore.Mvc;
using ShampanExam.Service.SetUp;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.SetUpVMs;

namespace ShampanExamAPI.Controllers.SetUp
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiscalYearDetailController : ControllerBase
    {
        // POST: api/FiscalYearDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(FiscalYearDetailVM fiscalYearDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            FiscalYearquestionSetDetailListervice _fiscalYearquestionSetDetailListervice = new FiscalYearquestionSetDetailListervice();

            try
            {
                resultVM = await _fiscalYearquestionSetDetailListervice.Insert(fiscalYearDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearDetail
                };
            }
        }

        // POST: api/FiscalYearDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(FiscalYearDetailVM fiscalYearDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearquestionSetDetailListervice _fiscalYearquestionSetDetailListervice = new FiscalYearquestionSetDetailListervice();
                resultVM = await _fiscalYearquestionSetDetailListervice.Update(fiscalYearDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearDetail
                };
            }
        }

        // POST: api/FiscalYearDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(FiscalYearDetailVM fiscalYearDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                FiscalYearquestionSetDetailListervice _fiscalYearquestionSetDetailListervice = new FiscalYearquestionSetDetailListervice();

                string?[] IDs = null;
                IDs = new string?[] { fiscalYearDetail.Id.ToString() };

                resultVM = await _fiscalYearquestionSetDetailListervice.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearDetail
                };
            }
        }

        // POST: api/FiscalYearDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(FiscalYearDetailVM fiscalYearDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearquestionSetDetailListervice _fiscalYearquestionSetDetailListervice = new FiscalYearquestionSetDetailListervice();
                resultVM = await _fiscalYearquestionSetDetailListervice.List(new[] { "M.Id" }, new[] { fiscalYearDetail.Id.ToString() }, null);
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

        // GET: api/FiscalYearDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(FiscalYearDetailVM fiscalYearDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearquestionSetDetailListervice _fiscalYearquestionSetDetailListervice = new FiscalYearquestionSetDetailListervice();
                resultVM = await _fiscalYearquestionSetDetailListervice.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/FiscalYearDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearquestionSetDetailListervice _fiscalYearquestionSetDetailListervice = new FiscalYearquestionSetDetailListervice();
                resultVM = await _fiscalYearquestionSetDetailListervice.Dropdown(); // Adjust if Dropdown requires a different method
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
