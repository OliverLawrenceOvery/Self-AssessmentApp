using SelfAssessmentService_WPF.Commands;
using SelfAssessmentService_WPF.State.Navigator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            UpdateCurrentViewCommand = new UpdateCurrentViewCommand();
            UpdateCurrentViewCommand.Execute(ViewType.Home);
        }

        public INavigator Navigator { get; set; } = new Navigator();

        public ICommand UpdateCurrentViewCommand { get; }
    }
}
