using SelfAssessmentService_WPF.Commands;
using SelfAssessmentService_WPF.State.Authenticator;
using SelfAssessmentService_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.State.Navigator
{
    public class Navigator : INavigator, INotifyPropertyChanged
    {
        private BaseViewModel _currentViewModel;
        private IAuthenticator _authenticator;

        public Navigator(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public BaseViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand UpdateCurrentViewCommand => new UpdateCurrentViewCommand(this, _authenticator);


    }
}
