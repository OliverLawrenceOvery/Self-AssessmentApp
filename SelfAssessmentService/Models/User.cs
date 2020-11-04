using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_Domain.Models
{
    public class User : IdentityObject
    {
        public string Username { get; set; }
        public string PasswordHashed { get; set; }
        public string Email { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
