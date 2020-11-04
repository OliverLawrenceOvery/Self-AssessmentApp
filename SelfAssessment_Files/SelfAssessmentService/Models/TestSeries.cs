using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class TestSeries : IdentityObject
    {
        public string TestSeriesName { get; set; }
        public IEnumerable<Test> TestList { get; set; }
    }
}
