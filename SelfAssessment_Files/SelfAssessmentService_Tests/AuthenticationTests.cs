using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.Authentication_Services;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Tests
{
    public class AuthenticationTests
    {
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private Mock<IAccountService> _mockAccountService;
        private IAuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockAccountService = new Mock<IAccountService>();
            _authenticationService = new AuthenticationService(_mockAccountService.Object, _mockPasswordHasher.Object);
        }

        [Test]
        public async Task LoginWithAccountThatExists_WithCorrectPassword()
        {
            //Set-up any dependencies to return correct values (assume everything else works).
            //So we mock, or pre-define how all our other services perform
            _mockPasswordHasher.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), "test1")).Returns(PasswordVerificationResult.Success);
            _mockAccountService.Setup(s => s.GetByUsername("test1")).ReturnsAsync(new Account() { User = new User { Username = "test1" } });
            Account account = await _authenticationService.Login("test1", "test1");
            Assert.AreEqual("test1", account.User.Username);
        }

        [Test]
        public void LoginWithIncorrectPassword_ThrowsException()
        {
            _mockPasswordHasher.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), "test1")).Returns(PasswordVerificationResult.Failed);
            _mockAccountService.Setup(s => s.GetByUsername("test1")).ReturnsAsync(new Account() { User = new User { Username = "test1" } });
            Exception exception = Assert.ThrowsAsync<Exception>(() => _authenticationService.Login("test1", "test1"));
            Assert.AreEqual("test1", exception.Message);
        }

        [Test]
        public void LoginWithNonExistingUsername()
        {
            _mockPasswordHasher.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), "test1")).Returns(PasswordVerificationResult.Failed);
            Exception exception = Assert.ThrowsAsync<Exception>(() => _authenticationService.Login("test1", "test1"));
            Assert.AreEqual("test1", exception.Message);
        }

        [Test]
        public async Task PasswordAndConfirmPasswordDoNotMatch()
        {
            RegistrationResult result = await _authenticationService.Register("test1", "testpass", "nottestpass");
            Assert.AreEqual(result, RegistrationResult.PasswordsDoNotMatch);
        }

        [Test]
        public async Task RegisterWithUsernameThatAlreadyExists()
        {
            _mockAccountService.Setup(s => s.GetByUsername("test1")).ReturnsAsync(new Account() { User = new User { Username = "test1" } });
            RegistrationResult result = await _authenticationService.Register("test1", "test1", "test1");
            Assert.AreEqual(result, RegistrationResult.UsernameAlreadyExists);
        }

        [Test]
        public async Task RegisterAccountSuccessfully()   //Note obviously change the username and password for subsequent testing
        {
            RegistrationResult result = await _authenticationService.Register("registerTest", "pass", "pass");
            Assert.AreEqual(result, RegistrationResult.Success);
        }
       
    }
}
