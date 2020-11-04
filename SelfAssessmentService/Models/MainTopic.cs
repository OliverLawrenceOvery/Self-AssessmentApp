using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class MainTopic : IdentityObject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<SubTopic> SubTopics { get; set; }
    }
}
