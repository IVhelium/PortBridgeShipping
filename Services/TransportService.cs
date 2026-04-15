using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class TransportService
    {
        public List<Transport> GetAllTransports()
        {
            using var db = new ApplicationDbContext();

            return db.Transports.ToList();
        }

        public Transport? CreateTransport(Transport transport)
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

            return db.Transports.FirstOrDefault(t => t.Id == transport.Id);
        }

        public Transport? UpdateTransport(Transport transport, int id)
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

        public bool DeleteTransport(int id)
        {
            using var db = new ApplicationDbContext();

            var transportExist = db.Transports.FirstOrDefault(t => t.Id == id);

            if (transportExist == null) return false;

            db.Transports.Remove(transportExist);
            db.SaveChanges();

            return true;
        }

        public bool Exist(int number)
        {
            using var db = new ApplicationDbContext();

            return db.Transports.Any(t => t.Id == number);
        }
    }
}
