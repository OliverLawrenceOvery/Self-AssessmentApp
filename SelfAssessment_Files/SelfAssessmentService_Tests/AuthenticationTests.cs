using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
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
        public void LoginWithAccountThatExists_WithCorrectPassword()
        {
            
        }

        [Test]
        public void LoginWithIncorrectPassword_ThrowsException()
        {

        }

        [Test]
        public void LoginWithNonExistingUsername()
        {

        }
    }
}
