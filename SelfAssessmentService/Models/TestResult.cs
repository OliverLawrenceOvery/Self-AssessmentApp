using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class TestResult : IdentityObject
    {
        public Account Account { get; set; }
        public Test Test { get; set; }
        public int Mark { get; set; }
    }
}
