using SelfAssessmentService_WPF.State.Navigator;
using SelfAssessmentService_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SelfAssessmentService_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            INavigator navigator = new Navigator();

            Window window = new MainWindow();
            window.DataContext = new MainViewModel(navigator);
            window.Show();
            base.OnStartup(e);
        }
    }
}
