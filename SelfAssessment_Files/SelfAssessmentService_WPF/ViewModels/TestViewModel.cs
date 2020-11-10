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





        #region Visibilities
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
        #endregion



        #region Take Test functionality
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

        public int CurrentQuestion = 0;

        public ICommand StartTestCommand => new DelegateCommand<object>(FuncToCall);
        private void FuncToCall(object context)
        {
            _ = UpdateQuestion();
        }
        public async Task UpdateQuestion()
        {
            if (SelectedTest == null) { MessageBox.Show("Please select a test to take"); }
            else
            {
                IQuestionService questionService = new QuestionDataService();
                ITestService testService = new TestDataService();
                Test selectedTest = await testService.Get(SelectedTest.Id);
                IEnumerable<Question> selectedTestQuestions = await questionService.GetAllQuestionsForGivenTestName(selectedTest.TestName);
                if (selectedTest.TotalMark != selectedTestQuestions.Select(q => q.QuestionMark).Sum()) { MessageBox.Show("This test needs more questions in order to be eligible to be taken."); }
                else
                {
                    SelectedTest = selectedTest;
                    QuestionName = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
                    QuestionText = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionText;
                    QuestionOptions = (selectedTest.Questions.ToList())[CurrentQuestion].QuestionOptions.ToList();
                    ListVisibility = Visibility.Collapsed;
                    DescriptionVisibility = Visibility.Collapsed;
                    MainVisibility = Visibility.Visible;
                    CreateQuestionVisibility = Visibility.Collapsed;
                }
            }
        }
        public ICommand NextQuestionCommand => new DelegateCommand<object>(FuncToCall2);
        private async void FuncToCall2(object context)
        {
            if (SelectedOption == null)
            {
                MessageBox.Show("Please choose an answer.");
                return;
            }
            if (SelectedOption.OptionText == (SelectedTest.Questions.ToList())[CurrentQuestion].CorrectAnswer)
            {
                TotalTestMark += (SelectedTest.Questions.ToList())[CurrentQuestion].QuestionMark;
            }
            CurrentQuestion++;
            if (CurrentQuestion <= SelectedTest.Questions.Count() - 1) { _ = UpdateQuestion(); }
            else
            {
                ListVisibility = Visibility.Visible;
                DescriptionVisibility = Visibility.Visible;
                MainVisibility = Visibility.Collapsed;
                CreateQuestionVisibility = Visibility.Visible;
                CurrentQuestion = 0;
                TestResult newTestResult = new TestResult() { Mark = TotalTestMark * 100 / SelectedTest.TotalMark };
                ITestResultService service = new TestResultService();
                await service.CreatePersonalTestResult(CurrentAccount.Id, SelectedTest.TestName, newTestResult);
            }
        }
        #endregion



        #region TestSeries functionality
        private string _newSeriesName;
        public string NewSeriesName
        {
            get { return _newSeriesName; }
            set { _newSeriesName = value; OnPropertyChanged(nameof(NewSeriesName)); }
        }

        public ICommand CreateNewSeriesCommand => new DelegateCommand<object>(FuncToCall5);
        private async void FuncToCall5(object context)
        {
            if (NewSeriesName == null || NewSeriesName == "") { MessageBox.Show("You must enter a series name."); }
            else
            {
                TestSeries checkTestSeries = Context.TestSeries.Where(t => t.TestSeriesName == NewSeriesName).FirstOrDefault();
                if (checkTestSeries == null)
                {
                    IDataService<TestSeries> service = new GenericDataService<TestSeries>();
                    await service.Create(new TestSeries() { TestSeriesName = NewSeriesName });
                    _ = GetAllTestSeries();
                    NewSeriesName = null;
                }
                else { MessageBox.Show("A test series by this name already exists."); }
            }
        }
        #endregion


        #region Customise Test functionality
        private string _newTestName;
        public string NewTestName
        {
            get { return _newTestName; }
            set { _newTestName = value; OnPropertyChanged(nameof(NewTestName)); }
        }

        private int _newTestMark;
        public int NewTestMark
        {
            get { return _newTestMark; }
            set { _newTestMark = value; OnPropertyChanged(nameof(NewTestMark)); }
        }

        private string _selectedSeriesForNewTest;
        public string SelectedSeriesForNewTest
        {
            get { return _selectedSeriesForNewTest; }
            set { _selectedSeriesForNewTest = value; OnPropertyChanged(nameof(SelectedSeriesForNewTest)); }
        }

        public ICommand CreateNewTestCommand => new DelegateCommand<object>(FuncToCall4);
        private async void FuncToCall4(object context)
        {
            if (NewTestMark == 0) { MessageBox.Show("Please enter a total mark for this test"); }
            else if (NewTestName == null || NewTestName == "") { MessageBox.Show("Please enter a test name"); }
            else if (SelectedSeriesForNewTest == null || SelectedSeriesForNewTest == "") { MessageBox.Show("Please select a test series to assign this test to"); }
            else
            {
                Test newTest = new Test()
                {
                    TestName = NewTestName,
                    TotalMark = NewTestMark,
                };

                ITestService testService = new TestDataService();
                Test createdTest = await testService.CreateNewTest(newTest, SelectedSeriesForNewTest);
                if (createdTest == null) { MessageBox.Show("A test already exists with this name."); }
                else
                {
                    AllTests = Context.Tests.Select(q => q.TestName).ToList();
                    NewTestName = null;
                    NewTestMark = 0;
                    SelectedSeriesForNewTest = null;
                }
            }
        }
        public ICommand DeleteTest => new DelegateCommand<object>(FuncToCall6);
        private async void FuncToCall6(object context)
        {
            if (SelectedTest == null) { MessageBox.Show("Please select a test to delete"); }
            else
            {
                ITestService service = new TestDataService();
                await service.Delete(SelectedTest.Id);
                TestList = Context.Tests.Where(q => q.TestName.Contains(SearchText)).ToList();
            }
        }
        #endregion




        #region New Question functionality
        private string _questionTextForNewTest;
        public string QuestionTextForNewTest
        {
            get { return _questionTextForNewTest; }
            set { _questionTextForNewTest = value; OnPropertyChanged(nameof(QuestionTextForNewTest)); }
        }

        private string _firstOptionForNewTest;
        public string FirstOptionForNewTest
        {
            get { return _firstOptionForNewTest; }
            set { _firstOptionForNewTest = value; OnPropertyChanged(nameof(FirstOptionForNewTest)); }
        }

        private string _secondOptionForNewTest;
        public string SecondOptionForNewTest
        {
            get { return _secondOptionForNewTest; }
            set { _secondOptionForNewTest = value; OnPropertyChanged(nameof(SecondOptionForNewTest)); }
        }

        private string _thirdOptionForNewTest;
        public string ThirdOptionForNewTest
        {
            get { return _thirdOptionForNewTest; }
            set { _thirdOptionForNewTest = value; OnPropertyChanged(nameof(ThirdOptionForNewTest)); }
        }

        private string _fourthOptionForNewTest;
        public string FourthOptionForNewTest
        {
            get { return _fourthOptionForNewTest; }
            set { _fourthOptionForNewTest = value; OnPropertyChanged(nameof(FourthOptionForNewTest)); }
        }

        private string _selectedTestForNewQuestion;
        public string SelectedTestForNewQuestion
        {
            get { return _selectedTestForNewQuestion; }
            set { _selectedTestForNewQuestion = value; OnPropertyChanged(nameof(SelectedTestForNewQuestion)); }
        }

        private string _correctAnswerForNewTest;
        public string CorrectAnswerForNewTest
        {
            get { return _correctAnswerForNewTest; }
            set { _correctAnswerForNewTest = value; OnPropertyChanged(nameof(CorrectAnswerForNewTest)); }
        }

        public ICommand CreateNewQuestionCommand => new DelegateCommand<object>(FuncToCall3);
        private async void FuncToCall3(object context)
        {
            if (QuestionTextForNewTest == null || QuestionTextForNewTest == "") { MessageBox.Show("Please populate the question text field"); }
            else if (FirstOptionForNewTest == null || FirstOptionForNewTest == "") { MessageBox.Show("Please populate the first option text field"); }
            else if (SecondOptionForNewTest == null || SecondOptionForNewTest == "") { MessageBox.Show("Please populate the second option text field"); }
            else if (ThirdOptionForNewTest == null || ThirdOptionForNewTest == "") { MessageBox.Show("Please populate the third option text field"); }
            else if (FourthOptionForNewTest == null || FourthOptionForNewTest == "") { MessageBox.Show("Please populate the fourth option text field"); }
            else if (SelectedTestForNewQuestion == null || SelectedTestForNewQuestion == "") { MessageBox.Show("Please select an associated test for this question"); }
            else if (CorrectAnswerForNewTest == null || CorrectAnswerForNewTest == "") { MessageBox.Show("Please populate the correct answer text field"); }
            else
            {
                Question newQuestion = new Question()
                {
                    QuestionText = QuestionTextForNewTest,
                    CorrectAnswer = CorrectAnswerForNewTest,
                    QuestionMark = 10,
                };

                IQuestionService questionService = new QuestionDataService();
                ITestService testService = new TestDataService();
                IEnumerable<Question> selectedTestQuestions = await questionService.GetAllQuestionsForGivenTestName(SelectedTestForNewQuestion);

                if (selectedTestQuestions.Count() > 0)
                {
                    if (selectedTestQuestions.ToList()[0].Test.TotalMark == selectedTestQuestions.Select(q => q.QuestionMark).Sum())
                    { MessageBox.Show("This test cannot support any more questions (maximum score reached)."); }
                    else
                    {
                        await questionService.CreateNewQuestion(newQuestion, SelectedTestForNewQuestion, FirstOptionForNewTest, SecondOptionForNewTest, ThirdOptionForNewTest, FourthOptionForNewTest);
                    }
                }
                else
                {
                    await questionService.CreateNewQuestion(newQuestion, SelectedTestForNewQuestion, FirstOptionForNewTest, SecondOptionForNewTest, ThirdOptionForNewTest, FourthOptionForNewTest);
                }
                QuestionTextForNewTest = null;
                FirstOptionForNewTest = null;
                SecondOptionForNewTest = null;
                ThirdOptionForNewTest = null;
                FourthOptionForNewTest = null;
                SelectedTestForNewQuestion = null;
                CorrectAnswerForNewTest = null;
            }
        }
        #endregion
    }
}
