using ShampanExam.ViewModel.CommonVMs;
using System.ComponentModel.DataAnnotations;

namespace ShampanExam.ViewModel.SetUpVMs
{
    public class EnumTypeVM : AuditVM
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Enum Type")]
        public string? EnumType { get; set; }
        

    }


}
