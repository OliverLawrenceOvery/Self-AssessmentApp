using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class Account : IdentityObject
    {
        public User User { get; set; }
        public IEnumerable<TestResult> Results { get; set; }
    }
}
