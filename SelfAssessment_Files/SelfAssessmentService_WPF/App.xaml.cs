using Microsoft.AspNet.Identity;
using SelfAssessmentService_Domain.Services.Authentication_Services;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using SelfAssessmentService_EntityFramework.CRUD_Services;
using SelfAssessmentService_WPF.State.Authenticator;
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
            IPasswordHasher passwordHasher = new PasswordHasher();
            IAccountService accountService = new AccountDataService();
            IAuthenticationService service = new AuthenticationService(accountService, passwordHasher);
            IAuthenticator authenticator = new Authenticator(service);
            INavigator navigator = new Navigator(authenticator);

            Window window = new MainWindow();
            window.DataContext = new MainViewModel(navigator, authenticator);
            window.Show();
            base.OnStartup(e);
        }
    }
}
