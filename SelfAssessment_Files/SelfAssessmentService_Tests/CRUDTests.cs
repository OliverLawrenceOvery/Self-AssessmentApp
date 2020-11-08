using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using SelfAssessmentService_EntityFramework;
using SelfAssessmentService_EntityFramework.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfAssessmentService_Tests
{
    public class CRUDTests
    {
        IDataService<User> userService = new GenericDataService<User>();
        IAccountService accountService = new AccountDataService();
        ITestService testService = new TestDataService();
        IQuestionService questionService = new QuestionDataService();
        ISubTopicService subTopicService = new SubTopicDataService();
        ITestResultService testResultService = new TestResultService();


        //Used for any test methods that require an existing account/user in database
        [SetUp]
        public void Setup()
        {
            User user = new User()
            {
                Username = "test2",
                PasswordHashed = "test2",
                Email = "test@gmail.com",
                DateJoined = DateTime.Now
            };

            Account account = new Account()
            {
                User = user,
            };

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                context.Accounts.Add(account);
                context.SaveChanges();
            }
        }

        //Remove every account/user except test1 which is used for other purposes
        [TearDown]
        public void TearDown()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                List<Account> testAccounts = context.Accounts.Where(a => a.User.Username != "test1").ToList();
                foreach (var account in testAccounts)
                {
                    context.Accounts.Remove(account);
                }

                List<User> testUsers = context.Users.Where(a => a.Username != "test1").ToList();
                foreach (var user in testUsers)
                {
                    context.Users.Remove(user);
                }

                List<QuestionOption> testQuestionOptions = context.QuestionOptions.ToList();
                foreach (var questionOption in testQuestionOptions)
                {
                    context.QuestionOptions.Remove(questionOption);
                }

                List<Question> testQuestions = context.Questions.Where(a => a.QuestionText == "TestQuestion1").ToList();
                foreach (var question in testQuestions)
                {
                    context.Questions.Remove(question);
                }

                List<Test> testTests = context.Tests.Where(a => a.TestName == "Test1").ToList();
                foreach (var test in testTests)
                {
                    context.Tests.Remove(test);
                }
                context.SaveChanges();
            }
        }




        #region AccountDataService Tests

        #region Create
        [Test]
        public async Task ACreatedAccountHasUserAndAccountSavedToDatabase()
        {
            User user = new User()
            {
                Username = "test3",
                PasswordHashed = "test3",
                Email = "test@gmail.com",
                DateJoined = DateTime.Now
            };

            Account account = new Account()
            {
                User = user,
            };

            await accountService.Create(account);
            Account retrievedAccount;

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                retrievedAccount = context.Accounts.Include(u => u.User).Where(a => a.User.Username == "test3").FirstOrDefault();
            }

            Assert.AreEqual("test3", retrievedAccount.User.Username);
            Assert.AreEqual("test3", retrievedAccount.User.PasswordHashed);
            Assert.AreEqual("test@gmail.com", retrievedAccount.User.Email);
        }
        #endregion

        #region Read
        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenRetrievingByID()
        {
            Account retrievedAccount = await accountService.Get(1164);

            Assert.AreEqual("test1", retrievedAccount.User.Username);
            Assert.AreEqual("olawrenceovery@gmail.com", retrievedAccount.User.Email);
        }

        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenReadingByUsername()
        {
            Account retrievedAccount = await accountService.GetByUsername("test2");

            Assert.AreEqual("test2", retrievedAccount.User.Username);
            Assert.AreEqual("test@gmail.com", retrievedAccount.User.Email);
        }

        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenReadingAndGettingList()
        {
            IEnumerable<Account> retrievedAccounts = await accountService.GetAll();

            Assert.AreEqual(2, retrievedAccounts.Count());
        }
        #endregion

        #region Update
        [Test]
        public async Task UpdateFunctionalityAllowsTheUpdatingOfUserInformation()
        {
            User updatedUser = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "changedemail@gmail.com"
            };

            User retrievedUpdatedUser = await userService.Update(1193, updatedUser);

            Assert.AreEqual("test1", retrievedUpdatedUser.Username);
            Assert.AreEqual("changedemail@gmail.com", retrievedUpdatedUser.Email);

            User updatedUserBack = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com"
            };

            User retrievedUpdatedUserBack = await userService.Update(1193, updatedUserBack);
            Assert.AreEqual("olawrenceovery@gmail.com", retrievedUpdatedUserBack.Email);
        }
        #endregion

        #region Delete
        [Test]
        public async Task CreatedAccountsAreDeletedWhenDeleteOperationCalled()
        {
            User user = new User()
            {
                Username = "test3",
                PasswordHashed = "test3",
                Email = "delete@gmail.com",
                DateJoined = DateTime.Now
            };

            Account account = new Account()
            {
                User = user,
            };

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                await context.Set<Account>().AddAsync(account);
                await context.SaveChangesAsync();
            }

            await accountService.Delete(account.Id);
            Account retrievedAccount;

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                retrievedAccount = context.Accounts.Where(a => a.User.Username == "test3").FirstOrDefault();
            }

            Assert.IsNull(retrievedAccount);
        }
        #endregion

        #endregion


        #region TestDataService Tests
        [Test]
        public async Task ReadFunctionalityRetrievesQuestionOptionAndCorrespondingQuestionAndTest()
        {
            Test test;
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                test = new Test() { TestName = "Test1" };
                Question question = new Question { Test = test, QuestionText = "TestQuestion1" };
                QuestionOption questionOption = new QuestionOption() { OptionText = "TestOption1", Question = question };
                await context.Set<Test>().AddAsync(test);
                await context.Set<Question>().AddAsync(question);
                await context.Set<QuestionOption>().AddAsync(questionOption);
                await context.SaveChangesAsync();
            }

            Test retrievedTest = await testService.Get(test.Id);
            Question retrievedQuestion = retrievedTest.Questions.ToList()[0];
            QuestionOption retrievedQuestionOption = retrievedQuestion.QuestionOptions.ToList()[0];

            Assert.AreEqual("Test1", retrievedTest.TestName);
            Assert.AreEqual("TestQuestion1", retrievedQuestion.QuestionText);
            Assert.AreEqual("TestOption1", retrievedQuestionOption.OptionText);
        }
        #endregion




        #region QuestionDataService Tests
        [Test]
        public async Task CreateNewQuestion_GivenQuestionTextAndAllQuestionOptions()
        {
            Question newQuestion = new Question()
            {
                QuestionText = "TestQuestion1",
                CorrectAnswer = "Option2",
                QuestionMark = 10,
            };

            Question createdQuestion = await questionService.CreateNewQuestion(newQuestion, "Test1", "Option1",
                "Option2", "Option3", "Option4");

            Assert.AreEqual("TestQuestion1", createdQuestion.QuestionText);
            Assert.AreEqual("Option1", createdQuestion.QuestionOptions.ToList()[0].OptionText);
            Assert.AreEqual("Option2", createdQuestion.QuestionOptions.ToList()[1].OptionText);
            Assert.AreEqual("Option3", createdQuestion.QuestionOptions.ToList()[2].OptionText);
            Assert.AreEqual("Option4", createdQuestion.QuestionOptions.ToList()[3].OptionText);
            Assert.AreEqual("Option2", createdQuestion.CorrectAnswer);
        }
        #endregion
    }
}