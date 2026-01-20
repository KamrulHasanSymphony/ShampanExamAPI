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
        public string? Name { get; set; }
        public string? MobileNo { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedAt { get; set; }
        public string? CreatedFrom { get; set; }

    }
}
