using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class SubTopic : IdentityObject
    {
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public MainTopic MainTopic { get; set; }
    }
}
