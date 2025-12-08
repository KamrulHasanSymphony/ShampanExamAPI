using ShampanExam.ViewModel.CommonVMs;
using System;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class ExamVM : AuditVM
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Date { get; set; }
        public int? SubjectId { get; set; }
        public TimeSpan? Time { get; set; }
        public int Duration { get; set; }
        public int TotalMark { get; set; }
        public int MarkObtain { get; set; }
        public int? GradeId { get; set; }
        public string? Remarks { get; set; }
        public bool IsExamByQuestionSet { get; set; }
        public int? QuestionSetId { get; set; }
        public int? ExamineeGroupId { get; set; }
        public int? ExamineeId { get; set; }
        public string? ExamineeName { get; set; }
        public bool IsExamMarksSubmitted { get; set; }

        public List<ExamExamineeVM> examExamineeList { get; set; }
        public List<ExamQuestionHeaderVM> examQuestionHeaderList { get; set; }
        public List<ExamQuestionOptionDetailVM> examQuestionOptionDetailList { get; set; }
        public List<ExamQuestionShortDetailVM> examQuestionShortDetailList { get; set; }

        public PeramModel PeramModel { get; set; }

        public ExamVM()
        {
            examExamineeList = new List<ExamExamineeVM>();
            examQuestionHeaderList = new List<ExamQuestionHeaderVM>();
            examQuestionOptionDetailList = new List<ExamQuestionOptionDetailVM>();
            examQuestionShortDetailList = new List<ExamQuestionShortDetailVM>();

            PeramModel = new PeramModel();
        }
    }
}
