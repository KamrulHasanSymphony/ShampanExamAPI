using ShampanExam.ViewModel.CommonVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class ExamQuestionOptionDetailVM 
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int ExamQuestionHeaderId { get; set; }
        public int QuestionHeaderId { get; set; }
        public int QuestionOptionDetailId { get; set; }
        public string? QuestionOption { get; set; }
        public bool? ExamineeAnswer { get; set; }
        public bool? QuestionAnswer { get; set; }

    }
}
