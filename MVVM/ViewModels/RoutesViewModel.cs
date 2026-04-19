using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using PortBridgeShipping.Core;
using PortBridgeShipping.MVVM.Models;
using PortBridgeShipping.Services;

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

                    };
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion
    }
}
