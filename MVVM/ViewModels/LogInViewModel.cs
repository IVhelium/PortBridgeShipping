using System;
using System.Windows;
using System.Windows.Input;
using PortBridgeShipping.Core;
using PortBridgeShipping.Services;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class LogInViewModel : ObservableObject
    {
        private readonly UserService _usersService;
        private readonly Action _onLoginSuccess;

        public LogInViewModel(UserService usersService, Action onLoginSuccess)
        {
            _usersService = usersService;
            _onLoginSuccess = onLoginSuccess;

            LoginCommand = new RelayCommand(obj => Login(), obj => CanAttemptAuth());
            RegisterCommand = new RelayCommand(obj => Register(), obj => CanAttemptAuth());
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // update command CanExecute
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // update command CanExecute
            }
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private bool CanAttemptAuth() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private void Login()
        {
            if (_usersService.ValidateCredentials(Username, Password))
            {
                Message = "Login successful.";
                // Clear sensitive data quickly
                Password = string.Empty;
                _onLoginSuccess?.Invoke();
            }
            else
            {
                // Show service error if available
                Message = _usersService.LastError ?? "Invalid username or password.";
                MessageBox.Show(Message, "Login failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Register()
        {
            // Validate password length client-side for clearer feedback
            if (Password == null || Password.Length < 8)
            {
                Message = "Password must be at least 8 characters.";
                MessageBox.Show(Message, "Registration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var created = _usersService.CreateUser(Username, Password);
                if (created)
                {
                    Message = "User created. You can now log in.";
                    MessageBox.Show(Message, "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Message = _usersService.LastError ?? "Registration failed.";
                    MessageBox.Show(Message, "Registration failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Message = _usersService.LastError ?? "Registration error.";
                MessageBox.Show(Message, "Registration error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
