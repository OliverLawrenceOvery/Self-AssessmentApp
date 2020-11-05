using Microsoft.AspNet.Identity;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Domain.Services.Authentication_Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private IAccountService _accountService;
        private IPasswordHasher _passwordHasher;

        public AuthenticationService(IAccountService accountService, IPasswordHasher passwordHasher)
        {
            _accountService = accountService;
            _passwordHasher = passwordHasher;
        }

        public Task<Account> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationResult> Register(string username, string password, string confirmPassword)
        {
            throw new NotImplementedException();
        }
    }
}
