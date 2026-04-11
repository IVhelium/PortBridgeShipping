using System.IO;
using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.MVVM.Models;
using Container = PortBridgeShipping.MVVM.Models.Container;

namespace PortBridgeShipping.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Container> Containers { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteSegment> RouteSegments { get; set; }
        public DbSet<Status> Statuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = Path.Combine(folder, "PortBridgeShipping");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string dbPath = Path.Combine(path, "portbridgeDb.db");      // C:\Users\{User}\AppData\Local\PortBridgeShipping\portbridgeDb.db

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
