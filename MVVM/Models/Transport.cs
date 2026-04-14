using TransportType = PortBridgeShipping.Core.Collections.Enums.TransportType;

namespace PortBridgeShipping.MVVM.Models
{
    public class Transport
    {
        public int Id { get; set; }
        public int TransportNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public TransportType TransportType { get; set; }
        public int Capacity { get; set; }   // Количество контейнеров

        // Segments
        public ICollection<RouteSegmentTransport> TransportSegments { get; set; } = new List<RouteSegmentTransport>();
    }
}
