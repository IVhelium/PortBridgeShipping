using System.Linq;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class TransportService
    {
        public List<Transport> GetAllTransports()
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Transports.ToList();
            }
            catch
            {
                return new List<Transport>();
            }
        }

        public Transport? CreateTransport(Transport transport)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var createTransport = new Transport
                {
                    TransportNumber = transport.TransportNumber,
                    Name = transport.Name,
                    TransportType = transport.TransportType,
                    Capacity = transport.Capacity
                };

                db.Transports.Add(createTransport);
                db.SaveChanges();

                return db.Transports.FirstOrDefault(t => t.Id == createTransport.Id);
            }
            catch
            {
                return null;
            }
        }

        public Transport? UpdateTransport(Transport transport, int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var transportExist = db.Transports.FirstOrDefault(t => t.Id == id);

                if (transportExist == null) return null;

                transportExist.TransportNumber = transport.TransportNumber;
                transportExist.Name = transport.Name;
                transportExist.TransportType = transport.TransportType;
                transportExist.Capacity = transport.Capacity;

                db.SaveChanges();

                return db.Transports.FirstOrDefault(t => t.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteTransport(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var transportExist = db.Transports.FirstOrDefault(t => t.Id == id);

                if (transportExist == null) return false;

                db.Transports.Remove(transportExist);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Fixed: check TransportNumber instead of Id
        public bool Exist(int number)
        {
            try
            {
                using var db = new ApplicationDbContext();
                return db.Transports.Any(t => t.TransportNumber == number);
            }
            catch
            {
                return false;
            }
        }
    }
}
