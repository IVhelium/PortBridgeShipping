using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    class StatusService
    {
        public List<Status> GetAllStatuses()
        {
            using var db = new ApplicationDbContext();

            return db.Statuses.ToList();
        }
    }
}
