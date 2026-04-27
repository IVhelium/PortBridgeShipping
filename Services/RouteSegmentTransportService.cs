using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class RouteSegmentTransportService
    {
        public List<RouteSegmentTransport> GetAllRouteSegmentTransports()
        {
            using var db = new ApplicationDbContext();

            return db.RouteSegmentTransports
                .AsNoTracking()
                .Include(rst => rst.Transport)
                .Include(rst => rst.RouteSegment)
                .ToList();
        }

        public List<Transport> GetTransportsBySegment(int segmenId)
        {
            using var db = new ApplicationDbContext();

            return db.RouteSegmentTransports
                .Where(rst => rst.RouteSegmentId == segmenId)
                .Include(rst => rst.Transport)
                .Select(rst => rst.Transport)
                .ToList();
        }

        public List<RouteSegment> GetRouteSegmentsByTransport(int transportId)
        {
            using var db = new ApplicationDbContext();

            return db.RouteSegmentTransports
                .Where(rst => rst.TransportId == transportId)
                .Include(rst => rst.RouteSegment)
                .Select(rst => rst.RouteSegment)
                .ToList();
        }

        public RouteSegmentTransport? AddTransportToSegment(RouteSegmentTransport routeSegmentTransport)
        {
            using var db = new ApplicationDbContext();

            var transportSegmentExist = db.RouteSegmentTransports
                                       .FirstOrDefault(rst => rst.RouteSegmentId == routeSegmentTransport.RouteSegmentId
                                       && rst.TransportId == routeSegmentTransport.TransportId);

            if (transportSegmentExist != null) return null;

            var transportSegment = new RouteSegmentTransport
            {
                RouteSegmentId = routeSegmentTransport.RouteSegmentId,
                TransportId = routeSegmentTransport.TransportId
            };

            db.RouteSegmentTransports.Add(transportSegment);
            db.SaveChanges();

            return transportSegment;
        }

        public RouteSegmentTransport? UpdateTransportFromSegment(RouteSegmentTransport routeSegmentTransport ,int segmentId, int transportId)
        {
            using var db = new ApplicationDbContext();

            var transportSegmentExist = db.RouteSegmentTransports
                                       .FirstOrDefault(rst => rst.RouteSegmentId == segmentId
                                       && rst.TransportId == transportId);

            if (transportSegmentExist != null) return null;

            transportSegmentExist.RouteSegmentId = routeSegmentTransport.RouteSegmentId;
            transportSegmentExist.TransportId = routeSegmentTransport.TransportId;

            db.SaveChanges();

            return db.RouteSegmentTransports
                     .FirstOrDefault(rst => rst.RouteSegmentId == segmentId
                     && rst.TransportId == transportId);
        }

        public bool DeleteTransportFromSegment(int segmentId, int transportId)
        {
            using var db = new ApplicationDbContext();

            var transportSegmentExist = db.RouteSegmentTransports
                                       .FirstOrDefault(rst => rst.RouteSegmentId == segmentId
                                       && rst.TransportId == transportId);

            if (transportSegmentExist == null) return false;

            db.RouteSegmentTransports.Remove(transportSegmentExist);
            db.SaveChanges();

            return true;
        }
    }
}
