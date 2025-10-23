using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.QuestionVM;
using System;
using System.Collections.Generic;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class GradeHeaderVM : AuditVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Remarks { get; set; }

        public PeramModel PeramModel { get; set; }

        public List<GradeDetailVM> gradeDetailList { get; set; }

        public GradeHeaderVM()
        {
            PeramModel = new PeramModel();
            gradeDetailList = new List<GradeDetailVM>();
        }
    }
}
