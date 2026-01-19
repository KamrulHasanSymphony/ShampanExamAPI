using ShampanExam.ViewModel.CommonVMs;
using System;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class QuestionSubjectVM : AuditVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameInBangla { get; set; }
        public string? Remarks { get; set; }

        public PeramModel PeramModel { get; set; }

        public List<QuestionChapterVM> chapterList { get; set; }

        public QuestionSubjectVM()
        {
            PeramModel = new PeramModel();
            chapterList = new List<QuestionChapterVM>();
        }
    }
}
