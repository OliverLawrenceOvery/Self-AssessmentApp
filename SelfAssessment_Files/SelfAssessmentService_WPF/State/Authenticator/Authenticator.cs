using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.Authentication_Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_WPF.State.Authenticator
{
    public class Authenticator : INotifyPropertyChanged, IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;

        public Authenticator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        private Account _currentAccount;
        public Account CurrentAccount
        {
            get
            {
                return _currentAccount;
            }
            private set
            {
                _currentAccount = value;
                OnPropertyChanged(nameof(CurrentAccount));
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public bool IsLoggedIn => CurrentAccount != null;

        public async Task<bool> Login(string username, string password)
        {
            bool success = true;
            try
            {
                CurrentAccount = await _authenticationService.Login(username, password);
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public void Logout()
        {
            CurrentAccount = null;
        }

        public Task<RegistrationResult> Register(string username, string password, string confirmPassword)
        {
            return _authenticationService.Register(username, password, confirmPassword);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
