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
        private readonly IAccountService _accountService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationService(IAccountService accountService, IPasswordHasher passwordHasher)
        {
            _accountService = accountService;
            _passwordHasher = passwordHasher;
        }

        public async Task<Account> Login(string username, string password)
        {
            Account storedAccount = await _accountService.GetByUsername(username);

            if (storedAccount == null)
            {
                throw new Exception(message:username);
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(storedAccount.User.PasswordHashed, password);

            if (passwordResult != PasswordVerificationResult.Success)
            {
                throw new Exception(message:username);
            }
            return storedAccount;
        }

        public async Task<RegistrationResult> Register(string username, string password, string confirmPassword)
        {
            RegistrationResult result = RegistrationResult.Success;
            if (password != confirmPassword)
            {
                return RegistrationResult.PasswordsDoNotMatch;
            }

            Account usernameAccount = await _accountService.GetByUsername(username);

            if (usernameAccount != null)
            {
                return RegistrationResult.UsernameAlreadyExists;
            }

            if (result == RegistrationResult.Success)
            {

                string hashedPassword = _passwordHasher.HashPassword(password);

                User user = new User()
                {
                    Username = username,
                    PasswordHashed = hashedPassword,
                    DateJoined = DateTime.Now
                };

                Account account = new Account()
                {
                    User = user
                };

                await _accountService.Create(account);
            }
            return result;
        }
    }
}
