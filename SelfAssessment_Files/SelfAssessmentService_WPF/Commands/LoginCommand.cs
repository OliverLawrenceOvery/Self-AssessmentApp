using SelfAssessmentService_WPF.State.Authenticator;
using SelfAssessmentService_WPF.State.Navigator;
using SelfAssessmentService_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.Commands
{
    public class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private LoginViewModel _loginViewModel;
        private readonly IAuthenticator _authenticator;
        private readonly INavigator _navigator;

        public LoginCommand(LoginViewModel loginViewModel, IAuthenticator authenticator, INavigator navigator)
        {
            _loginViewModel = loginViewModel;
            _authenticator = authenticator;
            _navigator = navigator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            bool success = await _authenticator.Login(_loginViewModel.Username, _loginViewModel.Password);
            if(success)
            {
                _navigator.CurrentViewModel = new HomeViewModel(_authenticator);
            }
        }
    }
}
