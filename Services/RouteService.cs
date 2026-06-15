using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class RouteService
    {
        public List<Route> GetAllRoutes()
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Routes
                       .AsNoTracking()
                       .Include(r => r.Segments)
                       .ToList();
            }
            catch
            {
                return new List<Route>();
            }
        }

        public Route? GetRouteById(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Routes
                       .Include(r => r.Segments)
                       .FirstOrDefault(r => r.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public Route? CreateRoute(Route route)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var createRoute = new Route { Name = route.Name };

                db.Routes.Add(createRoute);
                db.SaveChanges();

                return db.Routes
                    .Include(r => r.Segments)
                    .FirstOrDefault(r => r.Id == createRoute.Id);
            }
            catch
            {
                return null;
            }
        }

        public Route? UpdateRoute(Route route, int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var routeExist = db.Routes
                                .Include(r => r.Segments)
                                .FirstOrDefault(r => r.Id == id);

                if (routeExist == null) return null;

                routeExist.Name = route.Name;

                db.SaveChanges();

                return db.Routes
                    .Include(r => r.Segments)
                    .FirstOrDefault(r => r.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteRoute(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var routeExist = db.Routes.FirstOrDefault(r => r.Id == id);

                if (routeExist == null) return false;

                db.Routes.Remove(routeExist);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Exists(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Routes.Any(r => r.Id == id);
            }
            catch
            {
                return false;
            }
        }
    }
}
