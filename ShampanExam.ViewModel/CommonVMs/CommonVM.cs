namespace ShampanExam.ViewModel.CommonVMs
{
    public class CommonVM
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? CompanyId { get; set; }
        public string? BranchId { get; set; }
        public string?[] IDs { get; set; } = Array.Empty<string?>();
        public string? ModifyBy { get; set; }
        public string? ModifyFrom { get; set; }
        public string? SettingValue { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? IsPost { get; set; }


        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? TableName { get; set; }
        public string? Name { get; set; }
        public string? Group { get; set; }
        public string? Value { get; set; }
        public string? QuestionSubjectId { get; set; }
        public string? QuestionType { get; set; }
        public string? NoOfQuestion { get; set; }
        public string? MultiOptionNo { get; set; }
        public string? SingleOptionNo { get; set; }
        public string? ExamineeId { get; set; }
        public string? ExamId { get; set; }
        public string? UserName { get; set; }

        public string[] ConditionalFields { get; set; } = Array.Empty<string>();
        public string[] ConditionalValues { get; set; } = Array.Empty<string>();

    }


}
