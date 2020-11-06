using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.Authentication_Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_WPF.State.Authenticator
{
    public interface IAuthenticator
    {
        Account CurrentAccount { get; }
        bool IsLoggedIn { get; }
        Task<RegistrationResult> Register(string username, string password, string confirmPassword);
        Task<bool> Login(string username, string password);
        void Logout();
    }
}
