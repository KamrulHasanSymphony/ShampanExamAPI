using ShampanExam.ViewModel.CommonVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class ExamExamineeVM 
    {
        public int Id { get; set; }
        public string? ExamId { get; set; }
        public int ExamineeId { get; set; }

    }
}
