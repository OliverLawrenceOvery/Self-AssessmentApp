using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class Test : IdentityObject
    {
        public string TestName { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public int TotalMark { get; set; }
        public TestSeries TestSeries { get; set; }
    }
}
