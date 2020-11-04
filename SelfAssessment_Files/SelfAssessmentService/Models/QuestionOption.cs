using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class QuestionOption : IdentityObject
    {
        public string OptionText { get; set; }
        public Question Question { get; set; }
    }
}
