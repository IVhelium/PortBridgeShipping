using PortBridgeShipping.Core;
using PortBridgeShipping.Core.Collections.Enums;
using PortBridgeShipping.MVVM.Models;
using PortBridgeShipping.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class RoutesViewModel : ObservableObject
    {
        #region Services

        private readonly RouteService _routeService = new();
        private readonly RouteSegmentService _routeSegmentService = new();
        private readonly TransportService _transportService = new();
        private readonly RouteSegmentTransportService _routeSegmentTransportService = new();

        #endregion

        public RoutesViewModel()
        {
            #region Commands Invocation

            AddCommand = new RelayCommand(Add, CanAdd);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            RemoveCommand = new RelayCommand(Remove, CanRemove);
            ClearCommand = new RelayCommand(ClearForm);

            #endregion

            Routes = new ObservableCollection<Route>();
            RouteSegments = new ObservableCollection<RouteSegment>();
            Transports = new ObservableCollection<Transport>();
            RouteSegmentTransports = new ObservableCollection<RouteSegmentTransport>();

            RoutesView = CollectionViewSource.GetDefaultView(Routes);
            RouteSegmentsView = CollectionViewSource.GetDefaultView(RouteSegments);

            ViewModes = Enum.GetValues<RouteViewMode>();

            LoadData();
        }


        #region Commands

        public RelayCommand AddCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        #endregion


        #region Collections

        private readonly ICollectionView RoutesView;
        private readonly ICollectionView RouteSegmentsView;

        public IEnumerable<RouteViewMode> ViewModes { get; set; }
        public ObservableCollection<Route> Routes { get; set; } = [];
        public ObservableCollection<RouteSegment> RouteSegments { get; set; } = [];
        public ObservableCollection<Transport> Transports { get; set; } = [];
        public ObservableCollection<RouteSegmentTransport> RouteSegmentTransports { get; set; } = [];

        #endregion


        #region Load Data Methods

        private void LoadData()
        {
            Routes.Clear();
            foreach (var r in _routeService.GetAllRoutes())
                Routes.Add(r);

            RouteSegments.Clear();
            foreach (var rs in _routeSegmentService.GetAllRouteSegments())
            {
                var route = Routes.FirstOrDefault(r => r.Id == rs.RouteId);
                if (route != null) rs.Route = route;

                RouteSegments.Add(rs);
            }

            Transports.Clear();
            foreach (var t in _transportService.GetAllTransports())
                Transports.Add(t);

            RouteSegmentTransports.Clear();
            foreach (var rst in _routeSegmentTransportService.GetAllRouteSegmentTransports())
                RouteSegmentTransports.Add(rst);
        }

        private void LoadSegmentsByRoute(int routeId)
        {
            RouteSegments.Clear();

            var segments = _routeSegmentService.GetAllRouteSegments()
                           .Where(rs => rs.RouteId == routeId)
                           .OrderBy(rs => rs.Order);

            foreach (var s in segments)
                RouteSegments.Add(s);
        }

        #endregion


        #region Properties

        // Edit Mode Value
        private RouteViewMode _selectedViewMode = RouteViewMode.Routes;
        public RouteViewMode SelectedViewMode
        {
            get { return _selectedViewMode; }
            set
            {
                _selectedViewMode = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }


        // Route Model Value
        private Route _route = new();
        public Route Route
        {
            get { return _route; }
            set
            {
                _route = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Selected Route Value
        private Route? _selectedRoute;
        public Route? SelectedRoute
        {
            get { return _selectedRoute; }
            set
            {
                _selectedRoute = value;
                OnPropertyChanged();

                if (_selectedRoute != null)
                {
                    Route = new Route
                    {
                        Id = _selectedRoute.Id,
                        Name = _selectedRoute.Name
                    };

                    LoadSegmentsByRoute(_selectedRoute.Id);
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // RouteSegment Model Value
        private RouteSegment _routeSegment = new();
        public RouteSegment RouteSegment
        {
            get { return _routeSegment; }
            set
            {
                _routeSegment = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Selected RouteSegment Value
        private RouteSegment? _selectedRouteSegmnet;
        public RouteSegment? SelectedRouteSegment
        {
            get { return _selectedRouteSegmnet; }
            set
            {
                _selectedRouteSegmnet = value;
                OnPropertyChanged();

                if (_selectedRouteSegmnet != null)
                {
                    RouteSegment = new RouteSegment
                    {
                        Id = _selectedRouteSegmnet.Id,
                        Order = _selectedRouteSegmnet.Order,
                        From = _selectedRouteSegmnet.From,
                        To = _selectedRouteSegmnet.To,
                        RouteId = _selectedRouteSegmnet.RouteId
                    };

                    SelectedRoute = Routes.FirstOrDefault(r => r.Id == _selectedRouteSegmnet.RouteId);
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Transport Model Value
        private Transport _transport = new();
        public Transport Transport
        {
            get { return _transport; }
            set
            {
                _transport = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Selected Transport Value
        private Transport? _selectedTranspor;
        public Transport? SelectedTrnasport
        {
            get { return _selectedTranspor; }
            set
            {
                _selectedTranspor = value;
                OnPropertyChanged();

                if (_selectedTranspor != null)
                {
                    Transport = new Transport
                    {
                        Id = _selectedTranspor.Id,
                        TransportNumber = _selectedTranspor.TransportNumber,
                        Name = _selectedTranspor.Name,
                        TransportType = _selectedTranspor.TransportType,
                        Capacity = _selectedTranspor.Capacity
                    };
                }
            }
        }

        #endregion


        #region Command Properties

        private bool CanAdd(object? parameter)
        {
            return SelectedViewMode switch
            {
                RouteViewMode.Routes => !string.IsNullOrWhiteSpace(Route.Name)
                                        && SelectedRoute == null
                                        && SelectedRouteSegment == null,

                RouteViewMode.RouteSegments => (RouteSegments.Count > 0 || !string.IsNullOrWhiteSpace(RouteSegment.From))
                                               && !string.IsNullOrWhiteSpace(RouteSegment.To)
                                               && RouteSegment.RouteId > 0
                                               && SelectedRoute != null
                                               && SelectedRouteSegment == null,

                _ => false
            };
        }

        private void Add(object? parameter)
        {
            if (SelectedViewMode == RouteViewMode.Routes)
            {
                Route route = new() { Name = Route.Name };

                var createdRoute = _routeService.CreateRoute(route);

                if (createdRoute != null) Routes.Add(createdRoute);
            }
            else
            {
                if (SelectedRoute == null) return;

                RouteSegment.RouteId = SelectedRoute.Id;

                RouteSegment routeSegment = new()
                {
                    From = RouteSegment.From,
                    To = RouteSegment.To,
                    RouteId = SelectedRoute.Id
                };

                var createdRouteSegment = _routeSegmentService.CreateRouteSegment(routeSegment);

                if (createdRouteSegment != null) RouteSegments.Add(createdRouteSegment);
            }

            ClearForm(null);
        }

        private bool CanUpdate(object? parameter)
        {
            return SelectedViewMode switch
            {
                RouteViewMode.Routes => SelectedRoute != null,

                RouteViewMode.RouteSegments => SelectedRoute != null
                                               && SelectedRouteSegment != null,

                _ => false
            };
        }

        private void Update(object? parameter)
        {
            if (SelectedViewMode == RouteViewMode.Routes)
            {
                if (SelectedRoute == null) return;

                Route route = new() { Name = Route.Name };

                var updatedRoute = _routeService.UpdateRoute(route, SelectedRoute.Id);

                // UI
                if (updatedRoute != null)
                {
                    int index = Routes.IndexOf(SelectedRoute);
                    if (index >= 0) Routes[index] = updatedRoute;
                    SelectedRoute = updatedRoute;
                }
            }
            else
            {
                if (SelectedRoute == null || SelectedRouteSegment == null) return;

                RouteSegment routeSegment = new()
                {
                    To = RouteSegment.To
                };

                var updatedRouteSegment = _routeSegmentService.UpdateRouteSegment(routeSegment, SelectedRouteSegment.Id);

                if (updatedRouteSegment != null)
                {
                    int index = RouteSegments.IndexOf(SelectedRouteSegment);
                    if (index >= 0) RouteSegments[index] = updatedRouteSegment;
                    SelectedRouteSegment = updatedRouteSegment;
                }

                LoadSegmentsByRoute(SelectedRoute.Id);
            }

            ClearForm(null);
        }

        private bool CanRemove(object? parameter)
        {
            return SelectedViewMode switch
            {
                RouteViewMode.Routes => SelectedRoute != null,

                RouteViewMode.RouteSegments => SelectedRoute != null
                                               && SelectedRouteSegment != null,

                _ => false
            };
        }

        private void Remove(object? parameter)
        {
            if (SelectedViewMode == RouteViewMode.Routes)
            {
                if (SelectedRoute != null)
                {
                    _routeService.DeleteRoute(SelectedRoute.Id);
                    Routes.Remove(SelectedRoute);
                }
            }
            else
            {
                if (SelectedRoute != null && SelectedRouteSegment != null)
                {
                    _routeSegmentService.DeleteRouteSegment(SelectedRouteSegment.Id);
                    RouteSegments.Remove(SelectedRouteSegment);

                    LoadSegmentsByRoute(SelectedRoute.Id);
                }
            }

            ClearForm(null);
        }

        private void ClearForm(object? parameter)
        {
            Route = new Route();
            RouteSegment = new RouteSegment();
            SelectedRoute = null;
            SelectedRouteSegment = null;
        }

        #endregion
    }
}
