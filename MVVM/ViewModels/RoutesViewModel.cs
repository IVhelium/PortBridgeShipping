using PortBridgeShipping.Core;
using PortBridgeShipping.MVVM.Models;
using PortBridgeShipping.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
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



            #endregion


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

        public ObservableCollection<Route> Routes { get; set; } = [];
        public ObservableCollection<RouteSegment> RouteSegments { get; set; } = [];
        public ObservableCollection<Transport> Transports { get; set; } = [];
        public ObservableCollection<RouteSegmentTransport> RouteSegmentTransports { get; set; } = [];

        #endregion


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


        #region Properties

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

                    Route = new Route
                    {
                        Id = _selectedRouteSegmnet.RouteId,
                        Name = _routeService.GetRouteById(_selectedRouteSegmnet.RouteId).ToString()
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

        #endregion


        #region Command Properties


        private bool CanSaveRoute(object? parameter)
        {
            return true;
        }

        private void SaveRoute(object? parameter)
        {

        }

        private bool CanAddSegment(object? parameter)
        {
            return true;
        }

        private void AddSegment(object? parameter)
        {

        }

        private bool CanUpdateSegment(object? parameter)
        {
            return true;
        }

        private void UpdateSegment(object? parameter)
        {

        }

        private bool CanRemoveSegment(object? parameter)
        {
            return true;
        }

        private void RemoveSegment(object? parameter)
        {

        }

        #endregion
    }
}
