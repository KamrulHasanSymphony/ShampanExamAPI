﻿using ShampanExam.ViewModel.CommonVMs;
using System;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class QuestionSetHeaderVM : AuditVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? QuestionHeaderId { get; set; }
        public int? TotalMark { get; set; }
        public string? Remarks { get; set; }

        public PeramModel PeramModel { get; set; }


        public List<QuestionSetDetailVM> questionSetDetailList { get; set; }
        //public List<QuestionSetQuestionVM> questionSetQuestionList { get; set; }

        public QuestionSetHeaderVM()
        {
            PeramModel = new PeramModel();
            questionSetDetailList = new List<QuestionSetDetailVM>();
            //questionSetQuestionList = new List<QuestionSetQuestionVM>();
        }
    }
}
