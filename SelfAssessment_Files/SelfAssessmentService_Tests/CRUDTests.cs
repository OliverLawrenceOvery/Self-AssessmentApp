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

            TestSeries testSeries = new TestSeries()
            {
                TestSeriesName = "TestTestSeries",
            };

            Test test = new Test()
            {
                TestSeries = testSeries,
                TestName = "TestName"
            };


            Test secondTest = new Test()
            {
                TestSeries = testSeries,
                TestName = "TestName2"
            };

            TestResult testResult = new TestResult()
            {
                Account = account,
                Mark = 30,
                Test = test
            };

            MainTopic mainTopic = new MainTopic()
            {
                Title = "MainTopic"
            };

            SubTopic subTopic = new SubTopic()
            {
                MainTopic = mainTopic,
                Title = "SubTopic"
            };

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                context.Accounts.Add(account);
                context.TestSeries.Add(testSeries);
                context.MainTopics.Add(mainTopic);
                context.SubTopics.Add(subTopic);
                context.Tests.Add(test);
                context.Tests.Add(secondTest);
                context.TestResults.Add(testResult);
                context.SaveChanges();
            }
        }

        //Remove every account/user except test1 which is used for other purposes
        [TearDown]
        public void TearDown()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                List<TestResult> testTestResults = context.TestResults.ToList();
                foreach (var testResult in testTestResults)
                {
                    context.TestResults.Remove(testResult);
                }

                List<QuestionOption> testQuestionOptions = context.QuestionOptions.ToList();
                foreach (var questionOption in testQuestionOptions)
                {
                    context.QuestionOptions.Remove(questionOption);
                }

                List<Question> testQuestions = context.Questions.ToList();
                foreach (var question in testQuestions)
                {
                    context.Questions.Remove(question);
                }

                List<Test> testTests = context.Tests.ToList();
                foreach (var test in testTests)
                {
                    context.Tests.Remove(test);
                }

                List<TestSeries> testTestSeries = context.TestSeries.ToList();
                foreach (var testSeries in testTestSeries)
                {
                    context.TestSeries.Remove(testSeries);
                }

                List<SubTopic> testSubTopics = context.SubTopics.ToList();
                foreach (var subTopic in testSubTopics)
                {
                    context.SubTopics.Remove(subTopic);
                }

                List<MainTopic> testMainTopics = context.MainTopics.ToList();
                foreach (var mainTopic in testMainTopics)
                {
                    context.MainTopics.Remove(mainTopic);
                }

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
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Account getAccountId = context.Accounts.Where(a => a.User.Username == "test2").FirstOrDefault();
                Account retrievedAccount = await accountService.Get(getAccountId.Id);
                Assert.AreEqual("test2", retrievedAccount.User.Username);
                Assert.AreEqual("test@gmail.com", retrievedAccount.User.Email);
            }
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
                Username = "test3",
                PasswordHashed = "test3",
                Email = "changedemail@gmail.com"
            };
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                User getUserId = context.Users.Where(u => u.Username == "test2").FirstOrDefault();
                await userService.Update(getUserId.Id, updatedUser);
            }

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                User retrievedUpdatedUser = context.Users.Where(u => u.Username == "test3").FirstOrDefault();
                Assert.AreEqual("test3", retrievedUpdatedUser.Username);
                Assert.AreEqual("changedemail@gmail.com", retrievedUpdatedUser.Email);
            }
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

                await accountService.Delete(account.Id);
                Account retrievedAccount = context.Accounts.Where(a => a.User.Username == "test3").FirstOrDefault();
                Assert.IsNull(retrievedAccount);
            }

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

        [Test]
        public async Task CreateNewTest_GivenTheTestSeriesName()
        {
            Test test = new Test()
            {
                TestName = "NewTest",
                TotalMark = 40
            };

            await testService.CreateNewTest(test, "TestTestSeries");

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Test createdTest = context.Tests.Where(t => t.TestName == "NewTest")
                    .Include(t => t.TestSeries)
                    .FirstOrDefault();
                Assert.AreEqual("TestTestSeries", createdTest.TestSeries.TestSeriesName);
                Assert.AreEqual("NewTest", createdTest.TestName);
            }
        }

        [Test]
        public async Task CreateNewTest_WithTestNameThatAlreadyExistsInDatabase()
        {
            Test test = new Test()
            {
                TestName = "TestName"
            };

            Test createdTest = await testService.CreateNewTest(test, "TestTestSeries");
            Assert.IsNull(createdTest);
        }

        [Test]
        public async Task DeleteTestAndAssociatedQuestions()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Test createdTest = context.Tests
                    .Where(t => t.TestName == "TestName")
                    .FirstOrDefault();
                bool result = await testService.Delete(createdTest.Id);
                Assert.IsTrue(result);
            }
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

            await questionService.CreateNewQuestion(newQuestion, "Test1", "Option1", "Option2", "Option3", "Option4");
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Question createdQuestion = context.Questions
                    .Include(q => q.QuestionOptions)
                    .Where(q => q.QuestionText == "TestQuestion1")
                    .FirstOrDefault();

                Assert.AreEqual("TestQuestion1", createdQuestion.QuestionText);
                Assert.AreEqual("Option1", createdQuestion.QuestionOptions.ToList()[0].OptionText);
                Assert.AreEqual("Option2", createdQuestion.QuestionOptions.ToList()[1].OptionText);
                Assert.AreEqual("Option3", createdQuestion.QuestionOptions.ToList()[2].OptionText);
                Assert.AreEqual("Option4", createdQuestion.QuestionOptions.ToList()[3].OptionText);
                Assert.AreEqual("Option2", createdQuestion.CorrectAnswer);
            }
        }
        #endregion


        #region SubTopicDataService Tests
        [Test]
        public async Task CreateANewSubTopicGivenAllNecessaryParameters()
        {
            await subTopicService.CreateNewSubTopic("MainTopic", "Title", "Intro", "Content", "Summary");
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                SubTopic subTopic = context.SubTopics.Where(s => s.Title == "Title").FirstOrDefault();
                Assert.AreEqual("Title", subTopic.Title);
                Assert.AreEqual("Intro", subTopic.Introduction);
            }
        }        

        [Test]
        public async Task UpdateSubTopicContents()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                SubTopic subTopic = context.SubTopics.Where(s => s.Title == "SubTopic").FirstOrDefault();
                SubTopic createdSubTopic = await subTopicService.UpdateSubTopic(subTopic.Id, "NewTitle", "NewIntro", "NewContent", "NewSummary");
            }

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                SubTopic retrievedsubTopic = context.SubTopics.Where(s => s.Title == "NewTitle").FirstOrDefault();
                Assert.AreEqual("NewTitle", retrievedsubTopic.Title);
                Assert.AreEqual("NewIntro", retrievedsubTopic.Introduction);
            }
        }
        #endregion


        #region TestResultService Tests
        [Test]
        public async Task CreateUniquePersonalTestResult()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Account account = context.Accounts.Where(a => a.User.Username == "test2").FirstOrDefault();
                Test test = context.Tests.Where(t => t.TestName == "TestName2").FirstOrDefault();
                TestResult testResult = new TestResult() { Mark = 10 };
                await testResultService.CreatePersonalTestResult(account.Id, test.TestName, testResult);

                TestResult retrievedTestResult = context.TestResults
                    .Include(t => t.Account)
                    .ThenInclude(t => t.User)
                    .Where(t => t.Mark == 10)
                    .Where(t => t.Test.TestName == "TestName2").FirstOrDefault();
                Assert.AreEqual(10, retrievedTestResult.Mark);
                Assert.AreEqual("test2", retrievedTestResult.Account.User.Username);
            }
        }

        [Test]
        public async Task CreateTestResult_ForAnAlreadyCompletedTest_WithAHigherMark()
        {

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Account account = context.Accounts.Where(a => a.User.Username == "test2").FirstOrDefault();
                Test test = context.Tests.Where(t => t.TestName == "TestName").FirstOrDefault();
                TestResult testResult = new TestResult() { Mark = 40 };
                await testResultService.CreatePersonalTestResult(account.Id, test.TestName, testResult);

                TestResult retrievedTestResult = context.TestResults
                    .Include(t => t.Account)
                    .ThenInclude(t => t.User)
                    .Where(t => t.Test.TestName == "TestName").FirstOrDefault();
                Assert.AreEqual(40, retrievedTestResult.Mark);
                Assert.AreEqual("test2", retrievedTestResult.Account.User.Username);
            }
        }

        [Test]
        public async Task CreateTestResult_ForAnAlreadyCompletedTest_WithALowerMark()
        {

            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Account account = context.Accounts.Where(a => a.User.Username == "test2").FirstOrDefault();
                Test test = context.Tests.Where(t => t.TestName == "TestName").FirstOrDefault();
                TestResult testResult = new TestResult() { Mark = 20 };
                await testResultService.CreatePersonalTestResult(account.Id, test.TestName, testResult);

                TestResult retrievedTestResult = context.TestResults
                    .Include(t => t.Account)
                    .ThenInclude(t => t.User)
                    .Where(t => t.Test.TestName == "TestName").FirstOrDefault();
                Assert.AreEqual(30, retrievedTestResult.Mark);  //We initialise the test result associated with this test with a mark of 30 in SetUp()
                Assert.AreEqual("test2", retrievedTestResult.Account.User.Username);
            }
        }

        [Test]
        public async Task GetAllPersonalTestResults()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Account account = context.Accounts.Where(a => a.User.Username == "test2").FirstOrDefault();
                List<TestResult> testResults = await testResultService.GetPersonalTestResults(account, "TestTestSeries");
                Assert.AreEqual(1, testResults.Count);
            }
        }
        #endregion
    }
}