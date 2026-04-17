using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using PortBridgeShipping.Core;
using PortBridgeShipping.Core.Collections.Enums;
using PortBridgeShipping.Core.Collections.Enums.Filters;
using PortBridgeShipping.MVVM.Models;
using PortBridgeShipping.Services;
using Container = PortBridgeShipping.MVVM.Models.Container;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class ContainersViewModel : ObservableObject
    {

        #region Services

        private readonly ContainerService _containerService = new();
        private readonly StatusService _statusService = new();
        public readonly RouteService _routeService = new();

        #endregion

        public ContainersViewModel()
        {
            #region Commands Invocation

            AddCommand = new RelayCommand(AddContainer, CanAddContainer);
            UpdateCommand = new RelayCommand(UpdateContainer, CanUpdateContainer);
            RemoveCommand = new RelayCommand(RemoveContainer, CanRemoveContainer);
            ClearCommand = new RelayCommand(ClearForm);

            #endregion

            Containers = new ObservableCollection<Container>();
            Container = new Container();

            ContainersView = CollectionViewSource.GetDefaultView(Containers);   // Создаем представление над основной коллекцией
            ContainersView.Filter = FilterContainers;                           // Присвоение метода логики фильтрации      

            ContainerTypes = Enum.GetValues<ContainerType>();
            Filters = Enum.GetValues<ContainerFilter>();

            LoadData();
        }


        #region Commands

        public RelayCommand AddCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        #endregion


        #region Collections

        private readonly ICollectionView ContainersView;

        public IEnumerable<ContainerType> ContainerTypes { get; set; } = Enum.GetValues<ContainerType>();   // Get all elements automaticaly
        public IEnumerable<ContainerFilter> Filters { get; set; } = Enum.GetValues<ContainerFilter>();
        public ObservableCollection<Status> Statuses { get; set; } = [];
        public ObservableCollection<Route> Routes { get; set; } = [];
        public ObservableCollection<Container> Containers { get; set; } = [];

        #endregion


        private void LoadData()
        {
            Statuses.Clear();
            foreach (var s in _statusService.GetAllStatuses())
                Statuses.Add(s);

            Routes.Clear();
            foreach (var r in _routeService.GetAllRoutes())
                Routes.Add(r);

            Containers.Clear();
            foreach (var c in _containerService.GetAllContainers())
            {
                var status = Statuses.FirstOrDefault(s => s.Id == c.StatusId);
                if (status != null) c.Status = status;

                var route = Routes.FirstOrDefault(r => r.Id == c.RouteId);
                if (route != null) c.Route = route;

                Containers.Add(c);
            }
        }


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
                    Container = new Container
                    {
                        Id = _selectedContainer.Id,
                        ContainerNumber = _selectedContainer.ContainerNumber,
                        ContainerWeight = _selectedContainer.ContainerWeight,
                        ContainerType = _selectedContainer.ContainerType,
                        StatusId = _selectedContainer.StatusId,
                        RouteId = _selectedContainer.RouteId
                    };
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // SearchBox Value
        private string? _searchBoxText;
        public string? SearchBoxText
        {
            get { return _searchBoxText; }
            set
            {
                _searchBoxText = value;
                OnPropertyChanged();

                ContainersView.Refresh();   // Обновление Вьев при каждом изменении данного свойства
            }
        }

        // ComboBox Filters Value
        private ContainerFilter _selectedFilter;
        public ContainerFilter SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();

                ContainersView.Refresh();   // Обновление Вьев при каждом изменении данного свойства
            }
        }

        #endregion


        #region Command Properties

        // AddCommand Property
        private bool CanAddContainer(object? parameter)
        {
            return Container.ContainerNumber > 0 &&
                   Container.ContainerWeight > 0 &&
                   Container.ContainerType != ContainerType.None &&
                   Container.StatusId > 0 && Container.RouteId > 0 &&
                   SelectedContainer == null &&
                   !_containerService.Exists(Container.ContainerNumber);
        }

        private void AddContainer(object? parameter)
        {
            Container container = new()
            {
                ContainerNumber = Container.ContainerNumber,
                ContainerWeight = Container.ContainerWeight,
                ContainerType = Container.ContainerType,
                StatusId = Container.StatusId,
                RouteId = Container.RouteId
            };

            var createdContainer = _containerService.CreateContainer(container);

            if (createdContainer != null) Containers.Add(createdContainer);

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

            Container container = new()
            {
                ContainerNumber = Container.ContainerNumber,
                ContainerWeight = Container.ContainerWeight,
                ContainerType = Container.ContainerType,
                StatusId = Container.StatusId,
                RouteId = Container.RouteId
            };

            //SelectedContainer.ContainerNumber = Container.ContainerNumber;
            //SelectedContainer.ContainerWeight = Container.ContainerWeight;
            //SelectedContainer.ContainerType = Container.ContainerType;
            //SelectedContainer.StatusId = Container.Status!.Id;
            //SelectedContainer.Status = Container.Status;
            //SelectedContainer.RouteId = Container.Route!.Id;
            //SelectedContainer.Route = Container.Route;

            var updatedContainer = _containerService.UpdateContainer(container, SelectedContainer.Id);

            // UI
            if (updatedContainer != null)
            {
                int index = Containers.IndexOf(SelectedContainer);
                if (index >= 0) Containers[index] = updatedContainer;
                SelectedContainer = updatedContainer;
            }

            ContainersView.Refresh();

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
                _containerService.DeleteContainer(SelectedContainer.Id);
                Containers.Remove(SelectedContainer);
            }

            // Clear
            SelectedContainer = null;
            Container = new Container();
        }


        // ClearCommand Property
        private void ClearForm(object? parameter)
        {
            SelectedContainer = null;
            Container = new Container();
            SearchBoxText = string.Empty;
            SelectedFilter = ContainerFilter.Number;
            ContainersView.Refresh();
        }

        #endregion


        #region Search Filters

        private bool FilterContainers(object? obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return true;    // Если серч бокс пустой просто игнорируем поиск

            var container = obj as Container;
            if (container == null) return false;

            return SelectedFilter switch
            {
                ContainerFilter.Number => container.ContainerNumber.ToString().Contains(SearchBoxText),
                ContainerFilter.Weight => container.ContainerWeight.ToString().Contains(SearchBoxText),
                ContainerFilter.Type => container.ContainerType.ToString().Contains(SearchBoxText),
                ContainerFilter.Status => container.Status.Name.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase),     // StringComparison.OrdinalIgnoreCase почти одно и то же что и .ToLower() но быстрее
                ContainerFilter.Route => container.Route.ToString().Contains(SearchBoxText),
                _ => true
            };
        }

        #endregion
    }
}
