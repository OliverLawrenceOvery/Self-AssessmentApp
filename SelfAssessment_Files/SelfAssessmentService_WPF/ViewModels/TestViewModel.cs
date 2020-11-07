using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using SelfAssessmentService_EntityFramework;
using SelfAssessmentService_EntityFramework.CRUD_Services;
using SelfAssessmentService_WPF.State.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        public SelfAssessmentDbContext Context => new SelfAssessmentDbContext();

        private IAuthenticator _authenticator;
        public Account CurrentAccount { get; set; }

        public TestViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            CurrentAccount = _authenticator.CurrentAccount;
            _ = GetAllTestSeries();
            ListVisibility = Visibility.Visible;
            DescriptionVisibility = Visibility.Visible;
            MainVisibility = Visibility.Collapsed;
            CreateQuestionVisibility = Visibility.Visible;
        }

        public async Task GetAllTestSeries()
        {
            IDataService<TestSeries> service = new GenericDataService<TestSeries>();
            AllTestSeries = (await service.GetAll()).Select(s => s.TestSeriesName).ToList();
        }

        private IEnumerable<string> _allTestSeries;
        public IEnumerable<string> AllTestSeries
        {
            get { return _allTestSeries; }
            set { _allTestSeries = value; OnPropertyChanged(nameof(AllTestSeries)); }
        }




        private Visibility _listVisibility;
        public Visibility ListVisibility
        {
            get { return _listVisibility; }
            set { _listVisibility = value; OnPropertyChanged(nameof(ListVisibility)); }
        }

        private Visibility _descriptionVisibility;
        public Visibility DescriptionVisibility
        {
            get { return _descriptionVisibility; }
            set { _descriptionVisibility = value; OnPropertyChanged(nameof(DescriptionVisibility)); }
        }

        private Visibility _mainVisibility;
        public Visibility MainVisibility
        {
            get { return _mainVisibility; }
            set { _mainVisibility = value; OnPropertyChanged(nameof(MainVisibility)); }
        }

        private Visibility _createQuestionVisibility;
        public Visibility CreateQuestionVisibility
        {
            get { return _createQuestionVisibility; }
            set { _createQuestionVisibility = value; OnPropertyChanged(nameof(CreateQuestionVisibility)); }
        }





        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                TestList = Context.Tests.Where(q => q.TestName.Contains(value)).ToList();
            }
        }

        private IList<Test> _testList;
        public IList<Test> TestList
        {
            get { return _testList; }
            set
            {
                if (string.IsNullOrEmpty(SearchText)) _testList = null;
                else _testList = value;
                OnPropertyChanged(nameof(TestList));
            }
        }

        private Test _selectedTest;
        public Test SelectedTest
        {
            get { return _selectedTest; }
            set { _selectedTest = value; OnPropertyChanged(nameof(SelectedTest)); }
        }





        public int CurrentQuestion = 0;
        public ICommand StartTestCommand => new DelegateCommand<object>(FuncToCall, FuncToEvaluate);
        private void FuncToCall(object context)
        {
            ListVisibility = Visibility.Collapsed;
            DescriptionVisibility = Visibility.Collapsed;
            MainVisibility = Visibility.Visible;
            CreateQuestionVisibility = Visibility.Collapsed;
            //Could be an overriden getbyId
            Test selectedTest = Context.Tests
               .Include(e => e.Questions)
               .ThenInclude(e => e.QuestionOptions)
               .Where(e => e.TestName == SelectedTest.TestName)
               .First();
            QuestionName = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
            QuestionText = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
            QuestionOptions = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionOptions.ToList();
        }

        private bool FuncToEvaluate(object context)
        {
            return true;
        }



        private string _questionName;
        public string QuestionName
        {
            get { return _questionName; }
            set { _questionName = value; OnPropertyChanged(nameof(QuestionName)); }
        }

        private string _questionText;
        public string QuestionText
        {
            get { return _questionText; }
            set { _questionText = value; OnPropertyChanged(nameof(QuestionText)); }
        }

        private IList<QuestionOption> _questionOptions;
        public IList<QuestionOption> QuestionOptions
        {
            get { return _questionOptions; }
            set { _questionOptions = value; OnPropertyChanged(nameof(QuestionOptions)); }
        }

        public QuestionOption SelectedOption { get; set; }

        public int TotalTestMark { get; set; } = 0;





        public string NewTestName { get; set; }
        public int NewTestMark { get; set; }
        public string SelectedSeriesForNewTest { get; set; }

        public ICommand CreateNewTestCommand => new DelegateCommand<object>(FuncToCall4, FuncToEvaluate4);
        private void FuncToCall4(object context)
        {
            using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
            {
                TestSeries retrievedSeries = db.TestSeries.Where(q => q.TestSeriesName == SelectedSeriesForNewTest)
                    .FirstOrDefault();
                Test newTest = new Test()
                {
                    TestName = NewTestName,
                    TotalMark = NewTestMark,
                    TestSeries = retrievedSeries
                };
                db.Tests.Add(newTest);
                db.SaveChanges();
            }
        }

        private bool FuncToEvaluate4(object context)
        {
            return true;
        }



        public string NewSeriesName { get; set; }

        public ICommand CreateNewSeriesCommand => new DelegateCommand<object>(FuncToCall5, FuncToEvaluate5);
        private void FuncToCall5(object context)
        {
            using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
            {
                TestSeries newTestSeries = new TestSeries()
                {
                    TestSeriesName = NewSeriesName
                };
                db.TestSeries.Add(newTestSeries);
                db.SaveChanges();
            }
        }

        private bool FuncToEvaluate5(object context)
        {
            return true;
        }





        public ICommand NextQuestionCommand => new DelegateCommand<object>(FuncToCall2, FuncToEvaluate2);
        private void FuncToCall2(object context)
        {
            Test selectedTest = Context.Tests
               .Include(e => e.Questions)
               .ThenInclude(e => e.QuestionOptions)
               .Where(e => e.TestName == SelectedTest.TestName)
               .First();
            if (SelectedOption == null)
            {
                MessageBox.Show("Please choose an answer");
                return;
            }

            if (SelectedOption.OptionText == (selectedTest.Questions.ToList())[CurrentQuestion].CorrectAnswer)
            {
                TotalTestMark += (selectedTest.Questions.ToList())[CurrentQuestion].QuestionMark;
            }
            CurrentQuestion++;
            if (CurrentQuestion <= selectedTest.Questions.Count() - 1)
            {
                QuestionName = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
                QuestionText = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
                QuestionOptions = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionOptions.ToList();
            }
            else
            {
                ListVisibility = Visibility.Visible;
                DescriptionVisibility = Visibility.Visible;
                MainVisibility = Visibility.Collapsed;
                CreateQuestionVisibility = Visibility.Visible;

                using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
                {
                    Account myAccount = db.Accounts.Where(a => a.Id == CurrentAccount.Id).FirstOrDefault();
                    Test currentTest = db.Tests.Where(e => e.TestName == SelectedTest.TestName).FirstOrDefault();
                    TestResult newTestResult = new TestResult()
                    {
                        Test = currentTest,
                        Account = myAccount,
                        Mark = TotalTestMark
                    };
                    db.TestResults.Add(newTestResult);
                    db.SaveChanges();
                }

            }
        }

        private bool FuncToEvaluate2(object context)
        {
            return true;
        }

        public IList<string> AllTests => Context.Tests.Select(q => q.TestName).ToList();


        public string QuestionTextForNewTest { get; set; }
        public string FirstOptionForNewTest { get; set; }
        public string SecondOptionForNewTest { get; set; }
        public string ThirdOptionForNewTest { get; set; }
        public string FourthOptionForNewTest { get; set; }
        public string SelectedTestForNewQuestion { get; set; }
        public string CorrectAnswerForNewTest { get; set; }

        public ICommand CreateNewQuestionCommand => new DelegateCommand<object>(FuncToCall3, FuncToEvaluate3);
        private void FuncToCall3(object context)
        {
            using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
            {
                Test retrievedTest = db.Tests.Where(q => q.TestName == SelectedTestForNewQuestion)
                    .FirstOrDefault();
                Question newQuestion = new Question()
                {
                    QuestionText = QuestionTextForNewTest,
                    CorrectAnswer = CorrectAnswerForNewTest,
                    QuestionMark = 10,
                    Test = retrievedTest
                };
                db.Questions.Add(newQuestion);
                db.QuestionOptions.Add(new QuestionOption { OptionText = FirstOptionForNewTest, Question = newQuestion });
                db.QuestionOptions.Add(new QuestionOption { OptionText = SecondOptionForNewTest, Question = newQuestion });
                db.QuestionOptions.Add(new QuestionOption { OptionText = ThirdOptionForNewTest, Question = newQuestion });
                db.QuestionOptions.Add(new QuestionOption { OptionText = FourthOptionForNewTest, Question = newQuestion });
                db.SaveChanges();
            }
        }

        private bool FuncToEvaluate3(object context)
        {
            return true;
        }
    }
}
