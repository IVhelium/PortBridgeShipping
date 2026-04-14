using System.IO;
using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.MVVM.Models;
using Container = PortBridgeShipping.MVVM.Models.Container;

namespace PortBridgeShipping.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Transport> Transports { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteSegment> RouteSegments { get; set; }
        public DbSet<RouteSegmentTransport> RouteSegmentTransports { get; set; }
        public DbSet<Status> Statuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(folder, "PortBridgeShippingDb");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string dbPath = Path.Combine(path, "portbridgeDb.db");      // C:\Users\{User}\MyDocuments\PortBridgeShipping\portbridgeDb.db

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Relations

            // Container -> Status
            modelBuilder.Entity<Container>()
                .HasOne(c => c.Status)
                .WithMany(s => s.Containers)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Container -> Route
            modelBuilder.Entity<Container>()
                .HasOne(c => c.Route)
                .WithMany(r => r.Containers)
                .HasForeignKey(c => c.RouteId)
                .OnDelete(DeleteBehavior.SetNull);

            // RouteSegment -> Route
            modelBuilder.Entity<RouteSegment>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.Segments)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // RouteSegmentTransport -> Transport
            modelBuilder.Entity<RouteSegmentTransport>()
                .HasOne(rst => rst.Transport)
                .WithMany(t => t.TransportSegments)
                .HasForeignKey(rst => rst.TransportId)
                .OnDelete(DeleteBehavior.Cascade);

            // RouteSegmentTransport -> RouteSegment
            modelBuilder.Entity<RouteSegmentTransport>()
                .HasOne(rst => rst.RouteSegment)
                .WithMany(rs => rs.SegmentTransports)
                .HasForeignKey(rst => rst.RouteSegmentId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion


            #region Unique/FK

            // Unique Container Number
            modelBuilder.Entity<Container>()
                .HasIndex(c => c.ContainerNumber)
                .IsUnique();

            // Unique RouteId and Order number
            modelBuilder.Entity<RouteSegment>()
                .HasIndex(rs => new { rs.RouteId, rs.Order })
                .IsUnique();

            // FK RouteSegmentId and TransportId
            modelBuilder.Entity<RouteSegmentTransport>()
                .HasKey(rst => new { rst.RouteSegmentId, rst.TransportId });

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
