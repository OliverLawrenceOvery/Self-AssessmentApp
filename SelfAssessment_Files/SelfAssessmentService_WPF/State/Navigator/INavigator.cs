using SelfAssessmentService_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.State.Navigator
{
    public enum ViewType
    {
        Home,
        Profile,
        Resources,
        Tests
    }

    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
        ICommand UpdateCurrentViewCommand { get; }
    }
}
