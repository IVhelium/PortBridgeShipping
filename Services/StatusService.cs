using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class StatusService
    {
        public List<Status> GetAllStatuses()
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Statuses.ToList();
            }
            catch
            {
                return new List<Status>();
            }
        }
    }
}
