using ShampanExam.ViewModel.CommonVMs;
using System;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class ExamineeGroupVM : AuditVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Remarks { get; set; }
        
        public PeramModel PeramModel { get; set; }

        public ExamineeGroupVM()
        {
            PeramModel = new PeramModel();
        }
    }
}
