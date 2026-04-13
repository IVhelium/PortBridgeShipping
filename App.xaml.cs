using System.Windows;
using Catel.Collections;
using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Core.Collections.Enums;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.Migrate();  // Create and update Db

                if (!db.Transports.Any())
                {
                    db.Transports.Add(
                        new Transport
                        {
                            TransportNumber = 123456789,
                            Name = "Name",
                            TransportType = TransportType.Ship,
                            Capacity = 15000
                        }
                    );
                }

                if (!db.Statuses.Any())
                {
                    db.Statuses.AddRange(
                        new Status { Name = "In stock" },
                        new Status { Name = "In Transit" },
                        new Status { Name = "Delayed" },
                        new Status { Name = "Delivered" }
                    );

                    db.SaveChanges();
                }

                if (!db.Routes.Any())
                {
                    var initialRoute = new Route { Name = "USA -> Europe" };

                    initialRoute.Segments.AddRange(
                    [
                        new RouteSegment { From = "Lisbon", To = "Madrid", TransportId = 1, Order = 1 },
                        new RouteSegment { From = "Madrid", To = "Port", TransportId = 1, Order = 2 },
                        new RouteSegment { From = "Port", To = "Egypt", TransportId = 1, Order = 3 }
                    ]);

                    db.Routes.Add(initialRoute);
                    db.SaveChanges();
                }
            }

            base.OnStartup(e);
        }
    }

}
