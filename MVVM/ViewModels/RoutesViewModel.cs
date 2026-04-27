using Catel.Collections;
using PortBridgeShipping.Core;
using PortBridgeShipping.Core.Collections.Enums;
using PortBridgeShipping.Core.Collections.Enums.Filters;
using PortBridgeShipping.MVVM.Models;
using PortBridgeShipping.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

            // Route RouteSegment
            RouteRouteSegment_AddCommand = new RelayCommand(RouteRouteSegmentAdd, RouteRouteSegmentCanAdd);
            RouteRouteSegment_UpdateCommand = new RelayCommand(RouteRouteSegmentUpdate, RouteRouteSegmentCanUpdate);
            RouteRouteSegment_RemoveCommand = new RelayCommand(RouteRouteSegmentRemove, RouteRouteSegmentCanRemove);
            RouteRouteSegment_ClearCommand = new RelayCommand(RouteRouteSegmentClearForm);

            // RouteSegment Transport
            RouteSegmentTransport_BindCommand = new RelayCommand(RouteSegmentTransportBind, RouteSegmentTransportCanBind);
            RouteSegmentTransport_UpdateCommand = new RelayCommand(RouteSegmentTransportUpdate, RouteSegmentTransportCanUpdate);
            RouteSegmentTransport_RemoveCommand = new RelayCommand(RouteSegmentTransportRemove, RouteSegmentTransportCanRemove);
            RouteSegmentTransport_ClearForm = new RelayCommand(RouteSegmentTransportClearForm);

            #endregion


            Routes = new ObservableCollection<Route>();
            RouteSegments = new ObservableCollection<RouteSegment>();
            Transports = new ObservableCollection<Transport>();
            RouteSegmentTransports = new ObservableCollection<RouteSegmentTransport>();


            #region Filters

            RoutesView = CollectionViewSource.GetDefaultView(Routes);
            RouteSegmentsView = CollectionViewSource.GetDefaultView(RouteSegments);
            TransportsView = CollectionViewSource.GetDefaultView(Transports);
            RouteSegmentTransportsView = CollectionViewSource.GetDefaultView(RouteSegmentTransports);
            RoutesView.Filter = FilterRoutes;
            RouteSegmentsView.Filter = FilterRouteSegments;
            TransportsView.Filter = FilterTransports;
            RouteSegmentTransportsView.Filter = FilterRouteSegmentTransports;

            #endregion

            Filters = Enum.GetValues<RouteFilter>();
            ViewModes = Enum.GetValues<RouteViewMode>();

            LoadData();
        }


        #region Commands

        // Route RouteSegment Commands
        public RelayCommand RouteRouteSegment_AddCommand { get; set; }
        public RelayCommand RouteRouteSegment_UpdateCommand { get; set; }
        public RelayCommand RouteRouteSegment_RemoveCommand { get; set; }
        public RelayCommand RouteRouteSegment_ClearCommand { get; set; }


        // RouteSegment Transport Commands
        public RelayCommand RouteSegmentTransport_BindCommand { get; set; }
        public RelayCommand RouteSegmentTransport_UpdateCommand { get; set; }
        public RelayCommand RouteSegmentTransport_RemoveCommand { get; set; }
        public RelayCommand RouteSegmentTransport_ClearForm { get; set; }

        #endregion


        #region Collections

        private readonly ICollectionView RoutesView;
        private readonly ICollectionView RouteSegmentsView;
        private readonly ICollectionView TransportsView;
        private readonly ICollectionView RouteSegmentTransportsView;

        public IEnumerable<RouteFilter> Filters { get; set; }
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

            UpdateFromField();
        }

        private void UpdateFromField()
        {
            var lastSegment = RouteSegments
                              .OrderBy(rs => rs.Order)
                              .LastOrDefault();

            if (lastSegment != null) RouteSegment.From = lastSegment.To;

            OnPropertyChanged(nameof(RouteSegment));
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
                OnPropertyChanged(nameof(SelectedRoute));   

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

        // RouteSegmentTransport Model Value
        private RouteSegmentTransport _routeSegmentTransport = new();
        public RouteSegmentTransport RouteSegmentTransport
        {
            get { return _routeSegmentTransport; }
            set
            {
                _routeSegmentTransport = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Selected RouteSegmentTransport Value
        private RouteSegmentTransport? _selectedRouteSegmentTransport;
        public RouteSegmentTransport? SelectedRouteSegmentTransport
        {
            get { return _selectedRouteSegmentTransport; }
            set
            {
                _selectedRouteSegmentTransport = value;
                OnPropertyChanged();

                if (_selectedRouteSegmentTransport != null)
                {
                    RouteSegmentTransport = new RouteSegmentTransport
                    {
                        RouteSegmentId = _selectedRouteSegmentTransport.RouteSegmentId,
                        TransportId = _selectedRouteSegmentTransport.TransportId
                    };
                }
            }
        }

        // SearchBox Value
        private string? _searchBoxText;
        public string? SearchBoxText
        {
            get { return _searchBoxText; }
            set
            {
                _searchBoxText = value;
                OnPropertyChanged();

                RefreshAllPages();
            }
        }

        // ComboBox Filters Value
        private RouteFilter _selectedFilter;
        public RouteFilter SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();

                RefreshAllPages();
            }
        }

        #endregion


        #region Command Properties

        #region Route RouteSegment

        private bool RouteRouteSegmentCanAdd(object? parameter)
        {
            return SelectedViewMode switch
            {
                RouteViewMode.Routes => !string.IsNullOrWhiteSpace(Route.Name)
                                        && SelectedRoute == null
                                        && SelectedRouteSegment == null,

                RouteViewMode.RouteSegments => (RouteSegments.Count > 0 || !string.IsNullOrWhiteSpace(RouteSegment.From))
                                               && !string.IsNullOrWhiteSpace(RouteSegment.To)
                                               && SelectedRoute != null
                                               && SelectedRouteSegment == null,

                _ => false
            };
        }

        private void RouteRouteSegmentAdd(object? parameter)
        {
            if (SelectedViewMode == RouteViewMode.Routes)
            {
                Route route = new() { Name = Route.Name };

                var createdRoute = _routeService.CreateRoute(route);

                if (createdRoute != null) Routes.Add(createdRoute);

                RouteRouteSegmentClearForm(null);
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

                if (createdRouteSegment != null)
                {
                    RouteSegments.Add(createdRouteSegment);
                    ClearFormSegments();
                    UpdateFromField();
                }

            }

            RefreshRoutesSegmentsPage();
        }

        private bool RouteRouteSegmentCanUpdate(object? parameter)
        {
            return SelectedViewMode switch
            {
                RouteViewMode.Routes => SelectedRoute != null,

                RouteViewMode.RouteSegments => SelectedRoute != null
                                               && SelectedRouteSegment != null,

                _ => false
            };
        }

        private void RouteRouteSegmentUpdate(object? parameter)
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

                RouteRouteSegmentClearForm(null);
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
                ClearFormSegments();
            }

            RefreshRoutesSegmentsPage();
        }

        private bool RouteRouteSegmentCanRemove(object? parameter)
        {
            return SelectedViewMode switch
            {
                RouteViewMode.Routes => SelectedRoute != null,

                RouteViewMode.RouteSegments => SelectedRoute != null
                                               && SelectedRouteSegment != null,

                _ => false
            };
        }

        private void RouteRouteSegmentRemove(object? parameter)
        {
            if (SelectedViewMode == RouteViewMode.Routes)
            {
                if (SelectedRoute != null)
                {
                    _routeService.DeleteRoute(SelectedRoute.Id);
                    Routes.Remove(SelectedRoute);
                    RouteSegments.Clear();

                    RouteRouteSegmentClearForm(null);
                }
            }
            else
            {
                if (SelectedRoute != null && SelectedRouteSegment != null)
                {
                    _routeSegmentService.DeleteRouteSegment(SelectedRouteSegment.Id);
                    RouteSegments.Remove(SelectedRouteSegment);

                    LoadSegmentsByRoute(SelectedRoute.Id);
                    ClearFormSegments();
                }
            }

            RefreshRoutesSegmentsPage();
        }

        private void RouteRouteSegmentClearForm(object? parameter)
        {
            Route = new Route();
            RouteSegment = new RouteSegment();
            SelectedRoute = null;
            SelectedRouteSegment = null;
        }

        #endregion


        #region RouteSegment Transport

        private bool RouteSegmentTransportCanBind(object? parameter)
        {
            return RouteSegmentTransport.RouteSegmentId > 0
                   && RouteSegmentTransport.TransportId > 0
                   && SelectedRouteSegmentTransport == null;
        }

        private void RouteSegmentTransportBind(object? parameter)
        {
            RouteSegmentTransport routeSegmentTransport = new()
            {
                RouteSegmentId = RouteSegment.Id,
                TransportId = Transport.Id
            };

            var createdSegmentTransport = _routeSegmentTransportService.AddTransportToSegment(routeSegmentTransport);

            if (createdSegmentTransport != null) RouteSegmentTransports.Add(createdSegmentTransport);
        } 

        private bool RouteSegmentTransportCanUpdate(object? parameter)
        {
            return SelectedRouteSegmentTransport != null;
        }

        private void RouteSegmentTransportUpdate(object? parameter)
        {
            if (SelectedRouteSegmentTransport == null) return;

            RouteSegmentTransport routeSegmentTransport = new()
            {
                RouteSegmentId = RouteSegment.Id,
                TransportId = Transport.Id
            };

            var updatedRouteSegmentTransport = _routeSegmentTransportService.UpdateTransportFromSegment(routeSegmentTransport, SelectedRouteSegmentTransport.RouteSegmentId, SelectedRouteSegmentTransport.TransportId);

            // UI
            if (updatedRouteSegmentTransport != null)
            {
                int index = RouteSegmentTransports.IndexOf(SelectedRouteSegmentTransport);
                if (index >= 0) RouteSegmentTransports[index] = updatedRouteSegmentTransport;
                SelectedRouteSegmentTransport = updatedRouteSegmentTransport;
            }
        }

        private bool RouteSegmentTransportCanRemove(object? parameter)
        {
            return SelectedRouteSegmentTransport != null;
        }

        private void RouteSegmentTransportRemove(object? parameter)
        {
            if (SelectedRouteSegmentTransport != null)
            {
                _routeSegmentTransportService.DeleteTransportFromSegment(SelectedRouteSegmentTransport.RouteSegmentId, SelectedRouteSegmentTransport.TransportId);
                RouteSegmentTransports.Remove(SelectedRouteSegmentTransport);
            }
        }

        private void RouteSegmentTransportClearForm(object? parameter)
        {
            SelectedRouteSegment = null;
            SelectedTrnasport = null;
            SelectedRouteSegmentTransport = null;
            RouteSegmentTransport = new RouteSegmentTransport();
        }

        #endregion

        #endregion


        #region Other Methods

        // Clear
        private void ClearFormSegments()
        {
            RouteSegment = new RouteSegment();
            SelectedRouteSegment = null;
        }

        // Refresh
        private void RefreshRoutesSegmentsPage()
        {
            RoutesView.Refresh();
            RouteSegmentsView.Refresh();
        }

        private void RefreshAllPages()
        {
            RoutesView.Refresh();
            RouteSegmentsView.Refresh();
            TransportsView.Refresh();
            RouteSegmentTransportsView.Refresh();
        }

        #endregion

        #region Search Filters

        private bool FilterRoutes(object? obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return true;    // Если серч бокс пустой просто игнорируем поиск

            var route = obj as Route;
            if (route == null) return false;

            return SelectedFilter switch
            {
                RouteFilter.RouteName => route.Name.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase),
                _ => true
            };
        }

        private bool FilterRouteSegments(object? obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return true;

            var routeSegment = obj as RouteSegment;
            if (routeSegment == null) return false;

            return SelectedFilter switch
            {
                RouteFilter.From => routeSegment.From.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase),
                RouteFilter.To => routeSegment.To.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase),
                RouteFilter.Route => routeSegment.RouteId.ToString().Contains(SearchBoxText),
                _ => true
            };
        }

        private bool FilterTransports(object? obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return true;

            var transport = obj as Transport;
            if (transport == null) return false;

            return SelectedFilter switch
            {
                RouteFilter.Number => transport.TransportNumber.ToString().Contains(SearchBoxText),
                RouteFilter.TransportName => transport.Name.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase),
                RouteFilter.Type => transport.TransportType.CompareTo(transport.TransportType).ToString().Contains(SearchBoxText),
                _ => true
            };
        }

        private bool FilterRouteSegmentTransports(object? obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return true;

            var segmentTransport = obj as RouteSegmentTransport;
            if (segmentTransport == null) return false;

            return SelectedFilter switch 
            {
                RouteFilter.Segment => segmentTransport.RouteSegmentId.ToString().Contains(SearchBoxText),
                RouteFilter.Transport => segmentTransport.TransportId.ToString().Contains(SearchBoxText),
                _ => true
            };
        }
        
        #endregion
    }
}
