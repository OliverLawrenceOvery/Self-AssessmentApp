using Prism.Commands;
using SelfAssessmentService_Domain.Services.Authentication_Services;
using SelfAssessmentService_WPF.Commands;
using SelfAssessmentService_WPF.State.Authenticator;
using SelfAssessmentService_WPF.State.Navigator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private IAuthenticator _authenticator;

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthenticator authenticator, INavigator navigator)
        {
            _authenticator = authenticator;
            LoginCommand = new LoginCommand(this, _authenticator, navigator);
            LoginVisibility = Visibility.Visible;
            RegisterVisibility = Visibility.Collapsed;
            ErrorMessageVisibility = Visibility.Collapsed;
        }

        private Visibility _loginVisibility;
        public Visibility LoginVisibility
        {
            get
            {
                return _loginVisibility;
            }
            set
            {
                _loginVisibility = value;
                OnPropertyChanged(nameof(LoginVisibility));
            }
        }

        private Visibility _registerVisibility;
        public Visibility RegisterVisibility
        {
            get
            {
                return _registerVisibility;
            }
            set
            {
                _registerVisibility = value;
                OnPropertyChanged(nameof(RegisterVisibility));
            }
        }

        private Visibility _errorMessageVisibility;
        public Visibility ErrorMessageVisibility
        {
            get
            {
                return _errorMessageVisibility;
            }
            set
            {
                _errorMessageVisibility = value;
                OnPropertyChanged(nameof(ErrorMessageVisibility));
            }
        }

        public ICommand RegisterCommand => new DelegateCommand<object>(FuncToCall, FuncToEvaluate);
        private void FuncToCall(object context)
        {
            LoginVisibility = Visibility.Collapsed;
            RegisterVisibility = Visibility.Visible;
        }

        private bool FuncToEvaluate(object context)
        {
            return true;
        }


        public ICommand GoBackCommand => new DelegateCommand<object>(FuncToCall2, FuncToEvaluate2);
        private void FuncToCall2(object context)
        {
            LoginVisibility = Visibility.Visible;
            RegisterVisibility = Visibility.Collapsed;
        }

        private bool FuncToEvaluate2(object context)
        {
            return true;
        }

        private string _newUsername;
        public string NewUsername
        {
            get
            {
                return _newUsername;
            }
            set
            {
                _newUsername = value;
                OnPropertyChanged(nameof(NewUsername));
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get
            {
                return _confirmNewPassword;
            }
            set
            {
                _confirmNewPassword = value;
                OnPropertyChanged(nameof(ConfirmNewPassword));
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        { 
            get
            {
                return _errorMessage;
            } 
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand CreateAccountCommand => new DelegateCommand<object>(FuncToCall3, FuncToEvaluate3);
        private async void FuncToCall3(object context)
        {
            RegistrationResult result = await _authenticator.Register(NewUsername, NewPassword, ConfirmNewPassword);
            if (result == RegistrationResult.PasswordsDoNotMatch)
            {
                ErrorMessage = "Passwords do not match";
                ErrorMessageVisibility = Visibility.Visible;
            }
            else if (result == RegistrationResult.UsernameAlreadyExists)
            {
                ErrorMessage = "This user already exists";
                ErrorMessageVisibility = Visibility.Visible;
            }
            else if(result == RegistrationResult.Success)
            {
                LoginVisibility = Visibility.Visible;
                RegisterVisibility = Visibility.Collapsed;
            }
        }

        private bool FuncToEvaluate3(object context)
        {
            return true;
        }

    }
}
