using ShampanExam.ViewModel.CommonVMs;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class GradeDetailVM : AuditVM
    {
        public int Id { get; set; }
        public int? GradeId { get; set; }
        public string? Grade { get; set; }
        public int? MinPercentage { get; set; }
        public decimal? MaxPercentage { get; set; }
        public decimal? GradePoint { get; set; }
        public string? GradePointNote { get; set; }

        public PeramModel PeramModel { get; set; }

        public GradeDetailVM()
        {
            PeramModel = new PeramModel();
        }
    }
}
