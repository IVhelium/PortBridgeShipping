using System.Windows;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    // Use EnsureCreated to support runtime without EF migrations files
                    try
                    {
                        db.Database.EnsureCreated();  // Create DB and schema if missing
                    }
                    catch
                    {
                        // fallback to Migrate if EnsureCreated fails for some reason
                        try { db.Database.Migrate(); } catch { }
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
                }
            }
            catch
            {
                // swallow startup DB errors to avoid crashing; consider logging in future
            }

            base.OnStartup(e);
        }
    }

}
