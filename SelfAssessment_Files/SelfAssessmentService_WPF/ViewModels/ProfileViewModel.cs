using Microsoft.EntityFrameworkCore;
using OxyPlot;
using OxyPlot.Axes;
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

namespace SelfAssessmentService_WPF.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public SelfAssessmentDbContext Context { get; set; } = new SelfAssessmentDbContext();

        private IAuthenticator _authenticator;
        public Account CurrentAccount { get; set; }

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        private IList<DataPoint> _points;
        public IList<DataPoint> Points
        {
            get {  return _points;  }
            set {  _points = value; OnPropertyChanged(nameof(Points)); }
        }
        public ProfileViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            CurrentAccount = _authenticator.CurrentAccount;
            PlotModel = new PlotModel() { Title = "New Graph" };
            Points = new List<DataPoint>();
        }
        public IEnumerable<TestResult> TestResults => Context.TestResults.Include(e => e.Test).Where(e => e.Account.Id == CurrentAccount.Id).ToList();
        public TestResult SelectedTestResult { get; set; }
        public IList<string> AllTests => Context.TestSeries.Select(q => q.TestSeriesName).ToList();



        private string _selectedSeries;
        public string SelectedSeries
        {
            get {  return _selectedSeries; }
            set
            {
                _selectedSeries = value;
                _ = GetPersonalTestResults();
            }
        }

        private List<TestResult> _personalTestResults;
        public List<TestResult> PersonalTestResults
        {
            get { return _personalTestResults; }
            set { _personalTestResults = value; OnPropertyChanged(nameof(PersonalTestResults)); }

        }
        public async Task GetPersonalTestResults()
        {
            ITestResultService service = new TestResultService();
            PersonalTestResults = await service.GetPersonalTestResults(CurrentAccount, SelectedSeries);
            Points = new List<DataPoint>();
            for (int i = 0; i < PersonalTestResults.Count; i++)
            {
                Points.Add(new DataPoint(i, PersonalTestResults[i].Mark));
            }
            MaximumY = PersonalTestResults.Count - 1;
        }

        private int _maximumY;
        public int MaximumY
        {
            get {  return _maximumY;  }
            set { _maximumY = value; OnPropertyChanged(nameof(MaximumY)); }
        }
    }
}
