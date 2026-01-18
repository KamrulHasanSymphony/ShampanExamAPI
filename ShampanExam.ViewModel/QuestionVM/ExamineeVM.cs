using ShampanExam.ViewModel.CommonVMs;
using System;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class ExamineeVM : AuditVM
    {
        public long Id { get; set; }
        public long? ExamineeId { get; set; }

        public int? ExamineeGroupId { get; set; }
        public string? Name { get; set; }
        public string? MobileNo { get; set; }
        public string? LogInId { get; set; }
        public string? Password { get; set; }
        public bool? IsChangePassword { get; set; }

        public PeramModel? PeramModel { get; set; }

        public ExamineeVM()
        {
            PeramModel = new PeramModel();
        }
    }
}
