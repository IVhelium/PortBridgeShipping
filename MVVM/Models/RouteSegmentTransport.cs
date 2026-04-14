namespace PortBridgeShipping.MVVM.Models
{
    public class RouteSegmentTransport
    {
        public int Id { get; set; }

        public int RouteSegmentId { get; set; }
        public RouteSegment RouteSegment { get; set; } = null!;

        public int TransoprtId { get; set; }
        public Transport Transport { get; set; } = null!;
    }
}
