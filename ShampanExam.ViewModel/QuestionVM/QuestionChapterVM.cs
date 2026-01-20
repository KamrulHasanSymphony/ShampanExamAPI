using ShampanExam.ViewModel.CommonVMs;
using System;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class QuestionChapterVM : AuditVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameInBangla { get; set; }
        public string? Remarks { get; set; }
        public int? QuestionSubjectId { get; set; }
        public string? QuestionSubjectName { get; set; }

        public PeramModel PeramModel { get; set; }

        public QuestionChapterVM()
        {
            PeramModel = new PeramModel();
        }
    }
}
