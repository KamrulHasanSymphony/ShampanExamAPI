using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class AutomatedExamDetailsVM
    {
        public int Id { get; set; }
        public int? AutomatedExamId { get; set; }
        public int? ExamineeId { get; set; }
        public int? SubjectId { get; set; }
        public int? NumberOfQuestion { get; set; }
        public string? QuestionType { get; set; }
        public decimal QuestionMark { get; set; }
        public string? SubjectName { get; set; }
        public string? ExamCode { get; set; }
        public string? ExamName { get; set; }
        public string? ExamType { get; set; }
        public string? SingleOption { get; set; }
        public int? SingleOptionNo { get; set; }
        public decimal? SingleQuestionMark { get; set; }
        public string? MultiOption { get; set; }
        public int? MultiOptionNo { get; set; }
        public decimal? MultiQuestionMark { get; set; }
    }
}
