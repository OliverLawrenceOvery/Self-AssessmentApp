using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_EntityFramework;
using SelfAssessmentService_WPF.State.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ListVisibility = Visibility.Visible;
            DescriptionVisibility = Visibility.Visible;
            MainVisibility = Visibility.Collapsed;
            CreateQuestionVisibility = Visibility.Visible;
        }

        private Visibility _listVisibility;
        public Visibility ListVisibility
        {
            get
            {
                return _listVisibility;
            }
            set
            {
                _listVisibility = value;
                OnPropertyChanged(nameof(ListVisibility));
            }
        }

        private Visibility _descriptionVisibility;
        public Visibility DescriptionVisibility
        {
            get
            {
                return _descriptionVisibility;
            }
            set
            {
                _descriptionVisibility = value;
                OnPropertyChanged(nameof(DescriptionVisibility));
            }
        }

        private Visibility _mainVisibility;
        public Visibility MainVisibility
        {
            get
            {
                return _mainVisibility;
            }
            set
            {
                _mainVisibility = value;
                OnPropertyChanged(nameof(MainVisibility));
            }
        }

        private Visibility _createQuestionVisibility;
        public Visibility CreateQuestionVisibility
        {
            get
            {
                return _createQuestionVisibility;
            }
            set
            {
                _createQuestionVisibility = value;
                OnPropertyChanged(nameof(CreateQuestionVisibility));
            }
        }


        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                QuizList = Context.Tests.Where(q => q.TestName.Contains(value)).ToList();
            }
        }

        private IList<Test> _quizList;
        public IList<Test> QuizList
        {
            get
            {
                return _quizList;
            }
            set
            {
                if (string.IsNullOrEmpty(SearchText)) _quizList = null;
                else _quizList = value;
                OnPropertyChanged(nameof(QuizList));
            }
        }

        private Test _selectedQuiz;
        public Test SelectedQuiz
        {
            get
            {
                return _selectedQuiz;
            }
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged(nameof(SelectedQuiz));
            }
        }

        public int CurrentQuestion = 0;
        public ICommand StartQuizCommand => new DelegateCommand<object>(FuncToCall, FuncToEvaluate);
        private void FuncToCall(object context)
        {
            ListVisibility = Visibility.Collapsed;
            DescriptionVisibility = Visibility.Collapsed;
            MainVisibility = Visibility.Visible;
            CreateQuestionVisibility = Visibility.Collapsed;
            Test selectedQuiz = Context.Tests
               .Include(e => e.Questions)
               .ThenInclude(e => e.QuestionOptions)
               .Where(e => e.TestName == SelectedQuiz.TestName)
               .First();
            QuestionName = (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionText;
            QuestionText = (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionText;
            QuestionOptions = (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionOptions.ToList();
        }

        private bool FuncToEvaluate(object context)
        {
            return true;
        }

        private string _questionName;
        public string QuestionName
        {
            get
            {
                return _questionName;
            }
            set
            {
                _questionName = value;
                OnPropertyChanged(nameof(QuestionName));
            }
        }

        private string _questionText;
        public string QuestionText
        {
            get
            {
                return _questionText;
            }
            set
            {
                _questionText = value;
                OnPropertyChanged(nameof(QuestionText));
            }
        }

        private IList<QuestionOption> _questionOptions;
        public IList<QuestionOption> QuestionOptions
        {
            get
            {
                return _questionOptions;
            }
            set
            {
                _questionOptions = value;
                OnPropertyChanged(nameof(QuestionOptions));
            }
        }

        public QuestionOption SelectedOption { get; set; }

        public int TotalTestMark { get; set; } = 0;

        public ICommand NextQuestionCommand => new DelegateCommand<object>(FuncToCall2, FuncToEvaluate2);
        private void FuncToCall2(object context)
        {
            Test selectedQuiz = Context.Tests
               .Include(e => e.Questions)
               .ThenInclude(e => e.QuestionOptions)
               .Where(e => e.TestName == SelectedQuiz.TestName)
               .First();
            if (SelectedOption == null)
            {
                MessageBox.Show("Please choose an answer");
                return;
            }

            if (SelectedOption.OptionText == (selectedQuiz.Questions.ToList())[CurrentQuestion].CorrectAnswer)
            {
                TotalTestMark += (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionMark;
            }
            CurrentQuestion++;
            if (CurrentQuestion <= selectedQuiz.Questions.Count() - 1)
            {
                QuestionName = (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionText;
                QuestionText = (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionText;
                QuestionOptions = (selectedQuiz.Questions.ToList())[CurrentQuestion].QuestionOptions.ToList();
            }
            else
            {
                ListVisibility = Visibility.Visible;
                DescriptionVisibility = Visibility.Visible;
                MainVisibility = Visibility.Collapsed;
                CreateQuestionVisibility = Visibility.Visible;
            }
        }

        private bool FuncToEvaluate2(object context)
        {
            return true;
        }

        public IList<string> AllQuizzes => Context.Tests.Select(q => q.TestName).ToList();


        public string QuestionTextForNewTest { get; set; }
        public string FirstOptionForNewTest { get; set; }
        public string SecondOptionForNewTest { get; set; }
        public string ThirdOptionForNewTest { get; set; }
        public string FourthOptionForNewTest { get; set; }
        public string SelectedSeriesForNewTest { get; set; }
        public string CorrectAnswerForNewTest { get; set; }

        public ICommand CreateNewTestCommand => new DelegateCommand<object>(FuncToCall3, FuncToEvaluate3);
        private void FuncToCall3(object context)
        {
            using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
            {
                Test retrievedQuiz = db.Tests.Where(q => q.TestName == SelectedSeriesForNewTest)
                    .FirstOrDefault();
                Question newQuestion = new Question()
                {
                    QuestionText = QuestionTextForNewTest,
                    CorrectAnswer = CorrectAnswerForNewTest,
                    QuestionMark = 10,
                    Test = retrievedQuiz
                };
                db.Questions.Add(newQuestion);
                db.SaveChanges();
                db.QuestionOptions.Add(new QuestionOption { OptionText = FirstOptionForNewTest, Question = newQuestion });
                db.SaveChanges();
                db.QuestionOptions.Add(new QuestionOption { OptionText = SecondOptionForNewTest, Question = newQuestion });
                db.SaveChanges();
                db.QuestionOptions.Add(new QuestionOption { OptionText = ThirdOptionForNewTest, Question = newQuestion });
                db.SaveChanges();
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
