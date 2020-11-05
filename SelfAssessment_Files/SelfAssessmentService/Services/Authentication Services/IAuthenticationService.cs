using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Domain.Services.Authentication_Services
{
    public enum RegistrationResult
    {
        Success,
        PasswordsDoNotMatch,
        UsernameAlreadyExists
    }
    public interface IAuthenticationService
    {
        Task<RegistrationResult> Register(string username, string password, string confirmPassword);

        Task<Account> Login(string username, string password);
    }
}
