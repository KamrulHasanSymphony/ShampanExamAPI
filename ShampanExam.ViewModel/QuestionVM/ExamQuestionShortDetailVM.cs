using ShampanExam.ViewModel.CommonVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class ExamQuestionShortDetailVM 
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int ExamQuestionHeaderId { get; set; }
        public int QuestionHeaderId { get; set; }
        public int QuestionShortDetailId { get; set; }
        public string? ExamineeAnswer { get; set; }
        public string? QuestionAnswer { get; set; }
    }
}
