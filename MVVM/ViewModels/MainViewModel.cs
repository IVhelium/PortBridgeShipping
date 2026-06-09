using System;
using System.Windows.Input;
using PortBridgeShipping.Core;
using PortBridgeShipping.Services;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Services

        private readonly TransportService _transportService = new();
        private readonly RouteService _routeService = new();
        private readonly ContainerService _containerService = new();
        private readonly UserService _userService = new();

        #endregion


        public MainViewModel()
        {
            ContainersVM = new ContainersViewModel();
            TransportsVM = new TransportsViewModel();
            RoutesVM = new RoutesViewModel();

            // Pass callback for successful login
            LogInVM = new LogInViewModel(_userService, OnLoggedIn);

            CurrentView = LogInVM;   // Start with login
            Title = "Welcome to Port Bridge Shipping";
            IsLoggedIn = false;

            #region Initialize commands to switch views

            ContainersViewCommand = new RelayCommand(
                obj =>
                {
                    CurrentView = ContainersVM;
                    Title = "Containers Management";
                },
                obj => IsLoggedIn && HasRoute()
            );

            TransportsViewCommand = new RelayCommand(obj =>
            {
                CurrentView = TransportsVM;
                Title = "Ships Management";
            }, obj => IsLoggedIn);

            RoutesViewCommand = new RelayCommand(
                obj =>
                {
                    CurrentView = RoutesVM;
                    Title = "Routes Management";
                },
                obj => IsLoggedIn && HasTransport()
            );

            LogInViewCommand = new RelayCommand(obj =>
            {
                CurrentView = LogInVM;
                Title = "Log In";
            });

            #endregion
        }


        #region Commands

        public RelayCommand ContainersViewCommand { get; set; }
        public RelayCommand TransportsViewCommand { get; set; }
        public RelayCommand RoutesViewCommand { get; set; }
        public RelayCommand LogInViewCommand { get; set; }

        #endregion


        #region Views

        public ContainersViewModel ContainersVM { get; set; }
        public TransportsViewModel TransportsVM { get; set; }
        public RoutesViewModel RoutesVM { get; set; }
        public LogInViewModel LogInVM { get; set; }

        #endregion


        #region Properties

        // Title switching property
        private string? _title;
        public string? Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        // View switching property
        private object? _currentView;
        public object? CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        // Verify LogIn property
        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();

                // Re-evaluate command CanExecute
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion


        #region Verification

        public bool HasTransport()
        {
            return _transportService.GetAllTransports().Count != 0;  // If Transport Count more than 0, return true
        }

        public bool HasRoute()
        {
            return _route_service_fallback();
        }

        // Helper - preserve original behavior while avoiding direct service usage mistakes
        private bool _route_service_fallback()
        {
            return _routeService.GetAllRoutes().Count != 0;
        }

        public bool HasContainer()
        {
            return _containerService.GetAllContainers().Count != 0;  // If Container Count more than 0, return true
        }

        #endregion

        private void OnLoggedIn()
        {
            IsLoggedIn = true;
            // After login, show containers page by default
            CurrentView = ContainersVM;
            Title = "Containers Management";
        }
    }
}
