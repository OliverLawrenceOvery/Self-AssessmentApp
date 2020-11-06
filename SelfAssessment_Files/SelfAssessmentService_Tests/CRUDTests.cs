using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        IAccountService accountService = new AccountDataService();
        IDataService<Test> testService = new TestDataService();
        IDataService<Question> questionService = new GenericDataService<Question>();
        IDataService<QuestionOption> questionOptionService = new GenericDataService<QuestionOption>();

        [SetUp]
        public void Setup()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                var rows = from o in context.Accounts
                           select o;
                foreach (var Account in rows)
                {
                    context.Accounts.Remove(Account);
                }

                var rows2 = from o in context.Users
                            select o;
                foreach (var user in rows2)
                {
                    context.Users.Remove(user);
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
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
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
                retrievedAccount = context.Accounts.Include(u => u.User).Where(a => a.User.Username == "test1").FirstOrDefault();
            }

            Assert.AreEqual("test1", retrievedAccount.User.Username);
            Assert.AreEqual("test1", retrievedAccount.User.PasswordHashed);
            Assert.AreEqual("olawrenceovery@gmail.com", retrievedAccount.User.Email);
        }
        #endregion

        #region Read
        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenRetrievingByID()
        {
            User user = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
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

            Account retrievedAccount = await accountService.Get(account.Id);

            Assert.AreEqual("test1", retrievedAccount.User.Username);
            Assert.AreEqual("test1", retrievedAccount.User.PasswordHashed);
            Assert.AreEqual("olawrenceovery@gmail.com", retrievedAccount.User.Email);
        }

        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenReadingByUsername()
        {
            User user = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
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

            Account retrievedAccount = await accountService.GetByUsername("test1");

            Assert.AreEqual("test1", retrievedAccount.User.Username);
            Assert.AreEqual("test1", retrievedAccount.User.PasswordHashed);
            Assert.AreEqual("olawrenceovery@gmail.com", retrievedAccount.User.Email);
        }

        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenReadingAndGettingList()
        {
            User user1 = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
                DateJoined = DateTime.Now
            };

            Account account1 = new Account()
            {
                User = user1,
            };

            User user2 = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
                DateJoined = DateTime.Now
            };

            Account account2 = new Account()
            {
                User = user2,
            };


            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                await context.Set<Account>().AddAsync(account1);
                await context.Set<Account>().AddAsync(account2);
                await context.SaveChangesAsync();
            }

            IEnumerable<Account> retrievedAccounts = await accountService.GetAll();

            Assert.AreEqual(2, retrievedAccounts.Count());
        }
        #endregion

        #region Update
        [Test]
        public async Task UpdateFunctionalityAllowsTheUpdatingOfUserInformation()
        {
            User user = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
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

            User updatedUser = new User()
            {
                Username = "test2",
                PasswordHashed = "test2",
                Email = "changedemail@gmail.com"
            };

            Account updatedAccount = new Account()
            {
                User = updatedUser
            };

            Account retrievedUpdatedAccount = await accountService.Update(account.Id, updatedAccount);

            Assert.AreEqual("test2", retrievedUpdatedAccount.User.Username);
            Assert.AreEqual("test2", retrievedUpdatedAccount.User.PasswordHashed);
            Assert.AreEqual("changedemail@gmail.com", retrievedUpdatedAccount.User.Email);
        }
        #endregion

        #region Delete
        [Test]
        public async Task CreatedAccountsAreDeletedWhenDeleteOperationCalled()
        {
            User user = new User()
            {
                Username = "test1",
                PasswordHashed = "test1",
                Email = "olawrenceovery@gmail.com",
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
                retrievedAccount = context.Accounts.Where(a => a.User.Username == "test1").FirstOrDefault();
            }

            Assert.IsNull(retrievedAccount);
        }
        #endregion

        #endregion


        #region TestDataService Tests
        [Test]
        public async Task ReadFunctionalityRetrievesAccountAndUserDataFromDatabase_WhenRetrievingByTestID()
        {
            Test test;
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                test = new Test() { TestName = "Test1" };
                Question question = new Question { Test = test, QuestionText = "Question1" };
                QuestionOption questionOption = new QuestionOption() { OptionText = "Option1", Question = question };
                await context.Set<Test>().AddAsync(test);
                await context.Set<Question>().AddAsync(question);
                await context.Set<QuestionOption>().AddAsync(questionOption);
                await context.SaveChangesAsync();
            }

            Test retrievedTest = await testService.Get(test.Id);
            Question retrievedQuestion = retrievedTest.Questions.ToList()[0];
            QuestionOption retrievedQuestionOption = retrievedQuestion.QuestionOptions.ToList()[0];

            Assert.AreEqual("Test1", retrievedTest.TestName);
            Assert.AreEqual("Question1", retrievedQuestion.QuestionText);
            Assert.AreEqual("Option1", retrievedQuestionOption.OptionText);
        }
        #endregion
    }
}