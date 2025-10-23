using ShampanExam.ViewModel.CommonVMs;
using System.ComponentModel.DataAnnotations;

namespace ShampanTailor.ViewModel.QuestionVM
{
    public class QuestionShortDetailVM : AuditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question Header is required")]
        public int QuestionHeaderId { get; set; }

        [Required(ErrorMessage = "Question Answer is required")]
        public string? QuestionAnswer { get; set; }

        public QuestionShortDetailVM()
        {
        }
    }
}
