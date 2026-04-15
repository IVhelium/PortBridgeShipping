namespace PortBridgeShipping.MVVM.Models
{
    public class RouteSegmentTransport
    {
        public int RouteSegmentId { get; set; }
        public RouteSegment RouteSegment { get; set; } = null!;

        public int TransportId { get; set; }
        public Transport Transport { get; set; } = null!;
    }
}
