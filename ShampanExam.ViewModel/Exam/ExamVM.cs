using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.Exam
{
    public class ExamVM
    {
        public List<QuestionVM> Questions { get; set; } = new List<QuestionVM>();
    }
    public class QuestionVM
    {
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionType { get; set; } // "Radio", "Checkbox", "Text"

        public int QuestionMark { get; set; }

        // For Radio and Checkbox questions
        public List<QuestionOptionVM> Options { get; set; } = new List<QuestionOptionVM>();

        // For Text questions
        public string CorrectAnswer { get; set; }

        // User's selected answer(s)
        public string UserAnswer { get; set; }
        public List<int> SelectedOptionIds { get; set; } = new List<int>();
    }

    // ViewModel for question options
    public class QuestionOptionVM
    {
        public int Id { get; set; }
        public int QuestionHeaderId { get; set; }
        public string QuestionOption { get; set; }
        public string QuestionAnswer { get; set; } // "True" or "False"
        public bool IsCorrect => QuestionAnswer?.ToLower() == "true";
    }
}
