using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using Container = PortBridgeShipping.MVVM.Models.Container;

namespace PortBridgeShipping.Services
{
    public class ContainerService
    {
        public List<Container> GetAllContainers()
        {
            using var db = new ApplicationDbContext();

            return db.Containers
                   .Include(c => c.Status)
                   .Include(c => c.Route)
                   .ToList();
        }

        public Container? GetContainerById(int id)
        {
            using var db = new ApplicationDbContext();

            var containerExist = db.Containers.FirstOrDefault(c => c.Id == id);
            if (containerExist == null) return null;

            return db.Containers.Find(id);
        }

        public Container? DeleteContainer(int id)
        {
            using var db = new ApplicationDbContext();

            var containerExist = db.Containers.FirstOrDefault(c => c.Id == id);
            if (containerExist == null) return null;

            db.Containers.Remove(containerExist);
            db.SaveChanges();

            return containerExist;
        }

        public Container? UpdateContainer(Container container, int id)
        {
            using var db = new ApplicationDbContext();

            var containerExist = db.Containers.FirstOrDefault(c => c.Id == id);
            if (containerExist == null) return null;

            containerExist.ContainerNumber = container.ContainerNumber;
            containerExist.ContainerWeight = container.ContainerWeight;
            containerExist.ContainerType = container.ContainerType;
            containerExist.Status = container.Status;
            containerExist.Route = container.Route;

            db.Containers.Update(containerExist);
            db.SaveChanges();

            return containerExist;
        }

        public Container CreateContainer(Container container)
        {
            using var db = new ApplicationDbContext();

            db.Containers.Add(container);
            db.SaveChanges();

            return container;
        }
    }
}
