using SelfAssessmentService_WPF.State.Authenticator;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private IAuthenticator _authenticator;

        public HomeViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }
    }
}
