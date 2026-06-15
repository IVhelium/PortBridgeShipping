using System;
using PortBridgeShipping.Core;
using PortBridgeShipping.Services;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        private readonly ContainerService _containerService = new();
        private readonly TransportService _transportService = new();
        private readonly RouteService _routeService = new();
        private readonly StatusService _statusService = new();

        public string[] Labels { get; private set; } = Array.Empty<string>();
        public int[] Values { get; private set; } = Array.Empty<int>();
        public Func<double, string> YFormatter { get; private set; }

        public StatisticsViewModel()
        {
            YFormatter = value => value.ToString("N0");
        }

        public void LoadStatistics()
        {
            try
            {
                var containers = _containerService.GetAllContainers()?.Count ?? 0;
                var transports = _transport_service_fallback();
                var routes = _route_service_fallback();
                var statuses = _status_service_fallback();

                Labels = new[] { "Containers", "Transports", "Routes", "Statuses" };
                Values = new[] { containers, transports, routes, statuses };

                OnPropertyChanged(nameof(Labels));
                OnPropertyChanged(nameof(Values));
                OnPropertyChanged(nameof(YFormatter));
            }
            catch
            {
                Labels = Array.Empty<string>();
                Values = Array.Empty<int>();
            }
        }

        private int _transport_service_fallback()
        {
            var list = _transportService.GetAllTransports();
            return list?.Count ?? 0;
        }

        private int _route_service_fallback()
        {
            var list = _routeService.GetAllRoutes();
            return list?.Count ?? 0;
        }

        private int _status_service_fallback()
        {
            var list = _statusService.GetAllStatuses();
            return list?.Count ?? 0;
        }
    }
}
