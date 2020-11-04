using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class Question : IdentityObject
    {
        public string QuestionText { get; set; }
        public IEnumerable<QuestionOption> QuestionOptions { get; set; }
        public string CorrectAnswer { get; set; }
        public int QuestionMark { get; set; }
        public Test Test { get; set; }
    }
}
