using SelfAssessmentService_WPF.State.Navigator;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public INavigator Navigator { get; set; } = new Navigator();
    }
}
