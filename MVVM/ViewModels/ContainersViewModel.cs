using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using PortBridgeShipping.Core;
using PortBridgeShipping.Core.Collections.Enums;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class ContainersViewModel : ObservableObject
    {
        public ContainersViewModel()
        {
            AddCommand = new RelayCommand(AddContainer, CanAddContainer);
            UpdateCommand = new RelayCommand(UpdateContainer, CanUpdateContainer);
            RemoveCommand = new RelayCommand(RemoveContainer, CanRemoveContainer);


            Container = new Container();

            // Test Data
            Statuses.Add(new Status { Id = 1, Name = "V puti" });
            Statuses.Add(new Status { Id = 2, Name = "Na Sklade" });
            Statuses.Add(new Status { Id = 3, Name = "Dostavlen" });

            Routes.Add(new Route
            {
                Id = 1,
                Name = "Europe -> Africa",
                Segments =
                [
                    new RouteSegment { From = "Lisbon", To = "Madrid", Transport = TransportType.Truck, Order = 1 },
                    new RouteSegment { From = "Madrid", To = "Port", Transport = TransportType.Truck, Order = 2 },
                    new RouteSegment { From = "Port", To = "Egypt", Transport = TransportType.Ship, Order = 3 }
                ]
            });

            ContainerTypes = new ObservableCollection<ContainerType>(Enum.GetValues<ContainerType>());
        }


        #region Commands

        public RelayCommand AddCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }

        #endregion


        #region Collections

        public ObservableCollection<ContainerType> ContainerTypes { get; set; } =
            new ObservableCollection<ContainerType>(Enum.GetValues<ContainerType>());   // Get all elements automaticaly
        public ObservableCollection<Status> Statuses { get; set; } = [];
        public ObservableCollection<Route> Routes { get; set; } = [];
        public ObservableCollection<Container> Containers { get; set; } = [];

        #endregion


        #region Properties

        // Container Model Value
        private Container _container = new();
        public Container Container
        {
            get { return _container; }
            set
            {
                _container = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Selected Container Value
        private Container? _selectedContainer;
        public Container? SelectedContainer
        {
            get { return _selectedContainer; }
            set
            {
                _selectedContainer = value;
                OnPropertyChanged();

                if (_selectedContainer != null)
                {
                    Container = _selectedContainer;
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion


        #region Command Properties

        // AddCommand Property
        private bool CanAddContainer(object? parameter)
        {
            return Container.ContainerNumber > 0 &&
                   Container.ContainerWeight > 0 &&
                   Container.ContainerType != default &&
                   Container.Status != null &&
                   Container.Route != null &&
                   SelectedContainer == null &&
                   !Containers.Any(con => con.ContainerNumber == Container.ContainerNumber);
        }

        private void AddContainer(object? parameter)
        {
            Container container = new()
            {
                ContainerNumber = Container.ContainerNumber,
                ContainerWeight = Container.ContainerWeight,
                ContainerType = Container.ContainerType,
                StatusId = Container.Status!.Id,
                Status = Container.Status,
                RouteId = Container.Route!.Id,
                Route = Container.Route
            };

            Containers.Add(container);

            // Clear
            Container = new Container();
            SelectedContainer = null;
        }


        // UpdateCommand Property
        private bool CanUpdateContainer(object? parameter)
        {
            return SelectedContainer != null;
        }

        private void UpdateContainer(object? parameter)
        {
            if (SelectedContainer == null) return;

            Container updateContainer = new()
            {
                ContainerNumber = Container.ContainerNumber,
                ContainerWeight = Container.ContainerWeight,
                ContainerType = Container.ContainerType,
                Status = Container.Status,
                Route = Container.Route
            };

            int index = Containers.IndexOf(SelectedContainer);
            Containers[index] = updateContainer;

            // Clear
            SelectedContainer = null;
            Container = new Container();
        }


        // RemoveCommand Property
        private bool CanRemoveContainer(object? parameter)
        {
            return SelectedContainer != null;
        }

        private void RemoveContainer(object? parameter)
        {
            if (SelectedContainer != null)
            {
                Containers.Remove(SelectedContainer);
            }

            Container = new Container();   // Clear
        }

        #endregion
    }
}
