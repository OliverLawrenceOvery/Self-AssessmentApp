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
        private INavigator _navigator;
        public UpdateCurrentViewCommand(INavigator navigator)
        {
            _navigator = navigator;
        }

        public event EventHandler CanExecuteChanged;

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
                    case ViewType.Home:
                        _navigator.CurrentViewModel = new HomeViewModel();
                        break;
                    case ViewType.Profile:
                        _navigator.CurrentViewModel = new ProfileViewModel();
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