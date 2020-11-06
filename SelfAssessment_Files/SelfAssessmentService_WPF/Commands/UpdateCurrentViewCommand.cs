using SelfAssessmentService_WPF.State.Authenticator;
using SelfAssessmentService_WPF.State.Navigator;
using SelfAssessmentService_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.Commands
{
    public class UpdateCurrentViewCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private INavigator _navigator;
        private IAuthenticator _authenticator;

        public UpdateCurrentViewCommand(INavigator navigator, IAuthenticator authenticator)
        {
            _navigator = navigator;
            _authenticator = authenticator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(parameter is ViewType)
            {
                ViewType viewType = (ViewType)parameter;
                switch (viewType)
                {
                    case ViewType.Login:
                        _navigator.CurrentViewModel = new LoginViewModel(_authenticator, _navigator);
                        break;
                    case ViewType.Home:
                        _navigator.CurrentViewModel = new HomeViewModel(_authenticator);
                        break;
                    case ViewType.Profile:
                        _navigator.CurrentViewModel = new ProfileViewModel(_authenticator);
                        break;
                    case ViewType.Resources:
                        _navigator.CurrentViewModel = new ResourceViewModel();
                        break;
                    case ViewType.Tests:
                        _navigator.CurrentViewModel = new TestViewModel();
                        break;
                }
            }
        }
    }
}