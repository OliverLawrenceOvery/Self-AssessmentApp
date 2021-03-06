﻿using SelfAssessmentService_WPF.Commands;
using SelfAssessmentService_WPF.State.Authenticator;
using SelfAssessmentService_WPF.State.Navigator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(INavigator navigator, IAuthenticator authenticator)
        {
            Authenticator = authenticator;
            Navigator = navigator;
            Navigator.UpdateCurrentViewCommand.Execute(ViewType.Login);
        }

        public INavigator Navigator { get; set; }
        public IAuthenticator Authenticator { get; }
    }

}
