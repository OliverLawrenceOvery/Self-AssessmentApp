using SelfAssessmentService_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_WPF.State.Navigator
{
    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
    }
}
