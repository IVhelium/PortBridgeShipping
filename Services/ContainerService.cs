using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
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
                   .AsNoTracking()
                   .Include(c => c.Status)
                   .Include(c => c.Route)
                   .ToList();
        }

        public Container? GetContainerById(int id)
        {
            using var db = new ApplicationDbContext();

            return db.Containers
                   .AsNoTracking()
                   .Include(c => c.Status)
                   .Include(c => c.Route)
                   .FirstOrDefault(c => c.Id == id);
        }

        public Container? CreateContainer(Container container)
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

        public Container? UpdateContainer(Container container, int id)
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

        public bool DeleteContainer(int id)
        {
            using var db = new ApplicationDbContext();

            var containerExist = db.Containers.FirstOrDefault(c => c.Id == id);
            if (containerExist == null) return false;

            db.Containers.Remove(containerExist);
            db.SaveChanges();

            return true;
        }

        public bool Exists(int number)
        {
            using var db = new ApplicationDbContext();

            return db.Containers.Any(c => c.ContainerNumber == number);
        }
    }
}
