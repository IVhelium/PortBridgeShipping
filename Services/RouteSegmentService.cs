using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class RouteSegmentService
    {
        public List<RouteSegment> GetAllRouteSegments()
        {
            using var db = new ApplicationDbContext();

            return db.RouteSegments
                    .AsNoTracking()
                    .Include(r => r.Route)
                    .OrderBy(r => r.RouteId)
                    .ThenBy(r => r.Order)
                    .ToList();
        }

        public RouteSegment? CreateRouteSegment(RouteSegment routeSegment)
        {
            using var db = new ApplicationDbContext();

            var segments = db.RouteSegments
                .Where(rs => rs.RouteId == routeSegment.RouteId)
                .OrderBy(rs => rs.Order)
                .ToList();

            var createRouteSegment = new RouteSegment
            {
                Order = segments.Count + 1,
                To = routeSegment.To,
                RouteId = routeSegment.RouteId
            };

            if (segments.Count == 0)
            {
                if (string.IsNullOrWhiteSpace(routeSegment.From)) return null;

                createRouteSegment.From = routeSegment.From;
            }
            else createRouteSegment.From = segments.Last().To;

            db.RouteSegments.Add(createRouteSegment);
            db.SaveChanges();

            return db.RouteSegments
                    .Include(r => r.Route)
                    .FirstOrDefault(rs => rs.Id == createRouteSegment.Id);
        }

        public RouteSegment? UpdateRouteSegment(RouteSegment routeSegment, int id)
        {
            using var db = new ApplicationDbContext();

            var routeSegmentExist = db.RouteSegments
                                    .Include(r => r.Route)
                                    .FirstOrDefault(rs => rs.Id == id);

            if (routeSegmentExist == null) return null;

            var segments = db.RouteSegments
                .Where(rs => rs.RouteId == routeSegment.RouteId)
                .OrderBy(rs => rs.Order)
                .ToList();

            if (routeSegmentExist.Order > 1)
            {
                var prevSegment = segments.First(rs => rs.Order == routeSegment.Order - 1);

                if (routeSegment.From != prevSegment.To) return null;
            }

            var nextSegment = segments.First(rs => rs.Order == routeSegment.Order + 1);

            if (nextSegment != null)
                if (nextSegment.From != routeSegment.To) return null;

            routeSegmentExist.From = routeSegment.From;
            routeSegmentExist.To = routeSegment.To;
            routeSegmentExist.RouteId = routeSegment.RouteId;

            db.SaveChanges();

            return db.RouteSegments
                    .Include(r => r.Route)
                    .FirstOrDefault(rs => rs.Id == id);
        }

        public bool DeleteRouteSegment(int id)
        {
            using var db = new ApplicationDbContext();

            var routeSegmentExist = db.RouteSegments.FirstOrDefault(rs => rs.Id == id);

            if (routeSegmentExist == null) return false;

            db.RouteSegments.Remove(routeSegmentExist);
            db.SaveChanges();

            var segments = db.RouteSegments
                .Where(rs => rs.RouteId == routeSegmentExist.RouteId)
                .OrderBy(rs => rs.Order)
                .ToList();

            // Логика переномерации сегментов после удаления
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].Order = i + 1;

                if (i > 0) segments[i].From = segments[i - 1].To;
            }

            db.SaveChanges();

            return true;
        }
    }
}
