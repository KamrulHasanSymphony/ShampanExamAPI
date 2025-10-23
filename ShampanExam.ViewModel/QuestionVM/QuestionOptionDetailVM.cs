using ShampanExam.ViewModel.CommonVMs;
using System.ComponentModel.DataAnnotations;

namespace ShampanTailor.ViewModel.QuestionVM
{
    public class QuestionOptionDetailVM : AuditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question Header is required")]
        public int QuestionHeaderId { get; set; }

        [Required(ErrorMessage = "Question Option is required")]
        [StringLength(50, ErrorMessage = "Question Option should not exceed 50 characters")]
        public string? QuestionOption { get; set; }

        [Required(ErrorMessage = "Question Answer is required")]
        public bool QuestionAnswer { get; set; }

        public QuestionOptionDetailVM()
        {
        }
    }
}
