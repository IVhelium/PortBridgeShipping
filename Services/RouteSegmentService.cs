using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class RouteSegmentService
    {
        private void ReOrderSegments(ApplicationDbContext db, int routeId)
        {
            var segments = db.RouteSegments
                           .Where(rs => rs.RouteId == routeId)
                           .OrderBy(rs => rs.Order)
                           .ToList();

            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].Order = i + 1;

                if (i > 0) segments[i].From = segments[i - 1].To;
            }
        }

        public List<RouteSegment> GetAllRouteSegments()
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.RouteSegments
                        .AsNoTracking()
                        .Include(r => r.Route)
                        .OrderBy(r => r.RouteId)
                        .ThenBy(r => r.Order)
                        .ToList();
            }
            catch
            {
                return new List<RouteSegment>();
            }
        }

        public RouteSegment? CreateRouteSegment(RouteSegment routeSegment)
        {
            try
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

                ReOrderSegments(db, createRouteSegment.RouteId);

                db.RouteSegments.Add(createRouteSegment);
                db.SaveChanges();

                return db.RouteSegments
                        .Include(r => r.Route)
                        .FirstOrDefault(rs => rs.Id == createRouteSegment.Id);
            }
            catch
            {
                return null;
            }
        }

        public RouteSegment? UpdateRouteSegment(RouteSegment routeSegment, int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var routeSegmentExist = db.RouteSegments
                                        .Include(r => r.Route)
                                        .FirstOrDefault(rs => rs.Id == id);

                if (routeSegmentExist == null) return null;

                routeSegmentExist.To = routeSegment.To;

                ReOrderSegments(db, routeSegmentExist.RouteId);

                db.SaveChanges();

                return db.RouteSegments
                        .Include(r => r.Route)
                        .FirstOrDefault(rs => rs.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteRouteSegment(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var routeSegmentExist = db.RouteSegments.FirstOrDefault(rs => rs.Id == id);

                if (routeSegmentExist == null) return false;

                db.RouteSegments.Remove(routeSegmentExist);

                ReOrderSegments(db, routeSegmentExist.RouteId);

                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
