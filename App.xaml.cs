using Catel.Collections;
using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;
using System.Windows;

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

                //if (!db.Routes.Any())
                //{
                //    var initialRoute = new Route { Name = "USA -> Europe" };

                //    initialRoute.Segments.AddRange(
                //    [
                //        new RouteSegment { From = "Lisbon", To = "Madrid", Order = 1 },
                //        new RouteSegment { From = "Madrid", To = "Port", Order = 2 },
                //        new RouteSegment { From = "Port", To = "Egypt", Order = 3 }
                //    ]);

                //    db.Routes.Add(initialRoute);
                //    db.SaveChanges();
                //}
            }

            base.OnStartup(e);
        }
    }

}
