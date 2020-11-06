using Microsoft.EntityFrameworkCore;
using OxyPlot;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_EntityFramework;
using SelfAssessmentService_WPF.State.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public SelfAssessmentDbContext Context { get; set; } = new SelfAssessmentDbContext();

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        private IAuthenticator _authenticator;
        public Account CurrentAccount { get; set; }

        public IList<TestResult> TestResults { get; set; }

        private IList<DataPoint> _points;
        public IList<DataPoint> Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Points));
            }
        }
        public ProfileViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            CurrentAccount = _authenticator.CurrentAccount;

            TestResults = Context.TestResults.Include(e => e.Test)
            .Where(e => e.Account.Id == CurrentAccount.Id).ToList();

            PlotModel = new PlotModel() { Title = "New Graph" };
            Points = new List<DataPoint>();
            for (int i = 0; i < 10; i++)
            {
                Points.Add(new DataPoint(i, i));
            }
        }

        public TestResult SelectedTestResult { get; set; }
        public IList<string> AllTests => Context.TestSeries.Select(q => q.TestSeriesName).ToList();

    }
}
