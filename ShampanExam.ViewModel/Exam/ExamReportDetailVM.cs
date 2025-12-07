using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.Exam
{
    public class ExamReportDetailVM
    {
        public int? Id { get; set; }
        public string? ExamineeName { get; set; }
        public string? ExamineeMobileNo { get; set; }
        public string? MarkObtain { get; set; }
        public string? Grade { get; set; }
        public string? TotalMark { get; set; }
        public string? Percentage { get; set; }
    }
}
