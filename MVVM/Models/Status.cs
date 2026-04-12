using System.Collections.ObjectModel;

namespace PortBridgeShipping.MVVM.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;


        public ICollection<Container> Containers { get; set; } = new List<Container>();

        public override string ToString()
        {
            return Name;
        }
    }
}
