using PortBridgeShipping.Core;
using PortBridgeShipping.Services;
using System.Windows.Input;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Services

        private readonly TransportService _transportService = new();
        private readonly RouteService _routeService = new();
        private readonly ContainerService _containerService = new();

        #endregion


        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            ContainersVM = new ContainersViewModel();
            ShipsVM = new ShipsViewModel();
            TrucksVM = new TrucksViewModel();
            RoutesVM = new RoutesViewModel();
            LogInVM = new LogInViewModel();


            CurrentView = LogInVM;   // Set initial page
            Title = "Welcome to Port Bridge Shipping";


            #region Initialize commands to switch views

            HomeViewCommand = new RelayCommand(obj =>
            {
                CurrentView = HomeVM;
                Title = "Home";
            });

            ContainersViewCommand = new RelayCommand(
                obj =>
                {
                    CurrentView = ContainersVM;
                    Title = "Containers Menegment";
                },
                obj => HasRoute()
            );

            ShipsViewCommand = new RelayCommand(obj =>
            {
                CurrentView = ShipsVM;
                Title = "Ships Menegment";
            });

            TrucksViewCommand = new RelayCommand(obj =>
            {
                CurrentView = TrucksVM;
                Title = "Trucks Menegment";
            });

            RoutesViewCommand = new RelayCommand(
                obj =>
                {
                    CurrentView = RoutesVM;
                    Title = "Routes Menegment";
                },
                obj => HasTransport()
            );

            LogInViewCommand = new RelayCommand(obj =>
            {
                CurrentView = LogInVM;
                Title = "Log In";
            });

            #endregion
        }


        #region Commands

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand ContainersViewCommand { get; set; }
        public RelayCommand ShipsViewCommand { get; set; }
        public RelayCommand TrucksViewCommand { get; set; }
        public RelayCommand RoutesViewCommand { get; set; }
        public RelayCommand LogInViewCommand { get; set; }

        #endregion


        #region Views

        public HomeViewModel HomeVM { get; set; }
        public ContainersViewModel ContainersVM { get; set; }
        public ShipsViewModel ShipsVM { get; set; }
        public TrucksViewModel TrucksVM { get; set; }
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
            return _routeService.GetAllRoutes().Count != 0;  // If Route Count more than 0, return true
        }

        public bool HasContainer()
        {
            return _containerService.GetAllContainers().Count != 0;  // If Container Count more than 0, return true
        }

        #endregion
    }
}
