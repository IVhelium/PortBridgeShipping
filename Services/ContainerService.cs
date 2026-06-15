using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using Container = PortBridgeShipping.MVVM.Models.Container;

namespace PortBridgeShipping.Services
{
    public class ContainerService
    {
        public List<Container> GetAllContainers()
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Containers
                       .AsNoTracking()
                       .Include(c => c.Status)
                       .Include(c => c.Route)
                       .ToList();
            }
            catch
            {
                return new List<Container>();
            }
        }

        public Container? GetContainerById(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Containers
                       .Include(c => c.Status)
                       .Include(c => c.Route)
                       .FirstOrDefault(c => c.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public Container? CreateContainer(Container container)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var createContainer = new Container
                {
                    ContainerNumber = container.ContainerNumber,
                    ContainerWeight = container.ContainerWeight,
                    ContainerType = container.ContainerType,
                    StatusId = container.StatusId,
                    RouteId = container.RouteId
                };

                db.Containers.Add(createContainer);
                db.SaveChanges();

                return db.Containers
                        .Include(s => s.Status)
                        .Include(r => r.Route)
                        .FirstOrDefault(c => c.Id == createContainer.Id);
            }
            catch
            {
                return null;
            }
        }

        public Container? UpdateContainer(Container container, int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var containerExist = db.Containers
                                    .Include(s => s.Status)
                                    .Include(r => r.Route)
                                    .FirstOrDefault(c => c.Id == id);

                if (containerExist == null) return null;

                containerExist.ContainerNumber = container.ContainerNumber;
                containerExist.ContainerWeight = container.ContainerWeight;
                containerExist.ContainerType = container.ContainerType;
                containerExist.StatusId = container.StatusId;
                containerExist.RouteId = container.RouteId;

                db.SaveChanges();

                return db.Containers
                       .Include(s => s.Status)
                       .Include(r => r.Route)
                       .FirstOrDefault(c => c.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteContainer(int id)
        {
            try
            {
                using var db = new ApplicationDbContext();

                var containerExist = db.Containers.FirstOrDefault(c => c.Id == id);

                if (containerExist == null) return false;

                db.Containers.Remove(containerExist);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Exists(int number)
        {
            try
            {
                using var db = new ApplicationDbContext();

                return db.Containers.Any(c => c.ContainerNumber == number);
            }
            catch
            {
                return false;
            }
        }
    }
}
