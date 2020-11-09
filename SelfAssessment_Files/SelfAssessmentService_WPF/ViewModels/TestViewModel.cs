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
            AllTests = Context.Tests.Select(q => q.TestName).ToList();
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


        private IEnumerable<string> _allTests;
        public IEnumerable<string> AllTests
        {
            get { return _allTests; }
            set { _allTests = value; OnPropertyChanged(nameof(AllTests)); }
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
        public ICommand StartTestCommand => new DelegateCommand<object>(FuncToCall);
        private void FuncToCall(object context)
        {
            ListVisibility = Visibility.Collapsed;
            DescriptionVisibility = Visibility.Collapsed;
            MainVisibility = Visibility.Visible;
            CreateQuestionVisibility = Visibility.Collapsed;
            _ = UpdateQuestion();
        }

        public async Task UpdateQuestion()
        {
            ITestService service = new TestDataService();
            Test selectedTest = await service.Get(SelectedTest.Id);
            SelectedTest = selectedTest;
            QuestionName = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
            QuestionText = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
            QuestionOptions = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionOptions.ToList();
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



        public ICommand CreateNewTestCommand => new DelegateCommand<object>(FuncToCall4);
        private async void FuncToCall4(object context)
        {
            Test newTest = new Test()
            {
                TestName = NewTestName,
                TotalMark = NewTestMark,
            };
            ITestService service = new TestDataService();
            Test createdTest = await service.CreateNewTest(newTest, SelectedSeriesForNewTest);
            if (createdTest == null)
            {
                MessageBox.Show("A test already exists with this name.");
            }
            else { AllTests = Context.Tests.Select(q => q.TestName).ToList(); }
        }

        public string NewSeriesName { get; set; }

        public ICommand CreateNewSeriesCommand => new DelegateCommand<object>(FuncToCall5);
        private async void FuncToCall5(object context)
        {
            TestSeries checkTestSeries = Context.TestSeries.Where(t => t.TestSeriesName == NewSeriesName).FirstOrDefault();
            if (checkTestSeries == null)
            {
                IDataService<TestSeries> service = new GenericDataService<TestSeries>();
                await service.Create(new TestSeries() { TestSeriesName = NewSeriesName });
                _ = GetAllTestSeries();
            }
            else { MessageBox.Show("A test series by this name already exists"); }
        }




        public ICommand NextQuestionCommand => new DelegateCommand<object>(FuncToCall2);
        private async void FuncToCall2(object context)
        {
            if (SelectedOption == null)
            {
                MessageBox.Show("Please choose an answer");
                return;
            }

            if (SelectedOption.OptionText == (SelectedTest.Questions.ToList())[CurrentQuestion].CorrectAnswer)
            {
                TotalTestMark += (SelectedTest.Questions.ToList())[CurrentQuestion].QuestionMark;
            }
            CurrentQuestion++;
            if (CurrentQuestion <= SelectedTest.Questions.Count() - 1)
            {
                _ = UpdateQuestion();
            }
            else
            {
                ListVisibility = Visibility.Visible;
                DescriptionVisibility = Visibility.Visible;
                MainVisibility = Visibility.Collapsed;
                CreateQuestionVisibility = Visibility.Visible;
                CurrentQuestion = 0;
                TestResult newTestResult = new TestResult() { Mark = TotalTestMark*100/SelectedTest.TotalMark };
                ITestResultService service = new TestResultService();
                await service.CreatePersonalTestResult(CurrentAccount.Id, SelectedTest.TestName, newTestResult);
            }
        }


        public string QuestionTextForNewTest { get; set; }
        public string FirstOptionForNewTest { get; set; }
        public string SecondOptionForNewTest { get; set; }
        public string ThirdOptionForNewTest { get; set; }
        public string FourthOptionForNewTest { get; set; }
        public string SelectedTestForNewQuestion { get; set; }
        public string CorrectAnswerForNewTest { get; set; }

        public ICommand CreateNewQuestionCommand => new DelegateCommand<object>(FuncToCall3);
        private async void FuncToCall3(object context)
        {
            using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
            {
                Question newQuestion = new Question()
                {
                    QuestionText = QuestionTextForNewTest,
                    CorrectAnswer = CorrectAnswerForNewTest,
                    QuestionMark = 10,
                };
                IQuestionService service = new QuestionDataService();
                await service.CreateNewQuestion(newQuestion, SelectedTestForNewQuestion, FirstOptionForNewTest
                    , SecondOptionForNewTest, ThirdOptionForNewTest, FourthOptionForNewTest);
            }
        }


    }
}
