using SelfAssessmentService_WPF.State.Navigator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.Commands
{
    public class UpdateCurrentViewCommand : ICommand
    {
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
            }
        }
    }
}
