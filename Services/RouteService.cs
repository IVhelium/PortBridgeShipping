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
    }
}
