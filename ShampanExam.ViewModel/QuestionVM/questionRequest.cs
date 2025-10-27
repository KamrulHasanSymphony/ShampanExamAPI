using ShampanExam.ViewModel.KendoCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanExam.ViewModel.QuestionVM
{
    public class questionRequest
    {
        public GridOptions Options { get; set; }
        public string ChapterID { get; set; }
    }
}
