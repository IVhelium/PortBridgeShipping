using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class RouteService
    {
        public List<Route> GetAllRoutes()
        {
            using var db = new ApplicationDbContext();

            return db.Routes
                   .Include(r => r.Segments)
                   .ToList();
        }

        public Route? CreateRoute(Route route)
        {
            using var db = new ApplicationDbContext();

            var createRoute = new Route { Name = route.Name };

            db.Routes.Add(createRoute);
            db.SaveChanges();

            return db.Routes
                .Include(r => r.Segments)
                .FirstOrDefault(r => r.Id == createRoute.Id);
        }

        public Route? UpdateRoute(Route route, int id)
        {
            using var db = new ApplicationDbContext();

            var routeExist = db.Routes
                            .Include(r => r.Segments)
                            .FirstOrDefault(r => r.Id == id);

            if (routeExist == null) return null;

            routeExist.Name = route.Name;

            return db.Routes
                .Include(r => r.Segments)
                .FirstOrDefault(r => r.Id == id);
        }

        public bool DeleteRoute(int id)
        {
            using var db = new ApplicationDbContext();

            var routeExist = db.Routes.FirstOrDefault(r => r.Id == id);

            if (routeExist == null) return false;

            db.Routes.Remove(routeExist);
            db.SaveChanges();

            return true;
        }

        public bool Exists(int id)
        {
            using var db = new ApplicationDbContext();

            return db.Routes.Any(r => r.Id == id);
        }
    }
}
