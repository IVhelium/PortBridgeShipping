using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using PortBridgeShipping.Core;
using PortBridgeShipping.Core.Collections.Enums;
using PortBridgeShipping.Core.Collections.Enums.Filters;
using PortBridgeShipping.MVVM.Models;
using PortBridgeShipping.Services;
using ObservableObject = PortBridgeShipping.Core.ObservableObject;

namespace PortBridgeShipping.MVVM.ViewModels
{
    public class TransportsViewModel : ObservableObject
    {
        #region Services

        private readonly TransportService _transportService = new();
        private readonly RouteSegmentTransportService _routeSegmentTransportService = new();

        #endregion

        public TransportsViewModel()
        {
            #region Commands Invocation

            AddCommand = new RelayCommand(AddTransport, CanAddTransport);
            UpdateCommand = new RelayCommand(UpdateTransport, CanUpdateTransport);
            RemoveCommand = new RelayCommand(RemoveTransport, CanRemoveTransport);
            ClearCommand = new RelayCommand(ClearForm);

            #endregion

            Transports = new ObservableCollection<Transport>();
            Transport = new Transport();

            TransportView = CollectionViewSource.GetDefaultView(Transports);
            TransportView.Filter = obj => FilterTransport(obj) && FilterTransportType(obj);

            TransportTypes = Enum.GetValues<TransportType>();
            Filters = Enum.GetValues<TransportFilter>();
            TransportFilterTypes = Enum.GetValues<TransportFilterType>();

            LoadData();
        }


        #region Commands

        public RelayCommand AddCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        #endregion


        #region Collections

        private readonly ICollectionView TransportView;

        public IEnumerable<TransportType> TransportTypes { get; set; }
        public IEnumerable<TransportFilter> Filters { get; set; }
        public IEnumerable<TransportFilterType> TransportFilterTypes { get; set; }
        public ObservableCollection<Transport> Transports { get; set; } = [];
        public ObservableCollection<RouteSegment> TransportSegments { get; set; } = [];

        #endregion


        #region Load Data Functions

        private void LoadData()
        {
            Transports.Clear();
            foreach (var t in _transportService.GetAllTransports())
                Transports.Add(t);
        }

        private void LoadTruckSegments()
        {
            TransportSegments.Clear();

            if (SelectedTransport == null) return;

            foreach (var rs in _routeSegmentTransportService.GetRouteSegmentsByTransport(SelectedTransport.Id))
                TransportSegments.Add(rs);
        }

        #endregion


        #region Properties

        // Truck Model Value
        private Transport _transport = new();
        public Transport Transport
        {
            get { return _transport; }
            set
            {
                _transport = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Selected Truck Value
        private Transport? _selectedTransport;
        public Transport? SelectedTransport
        {
            get { return _selectedTransport; }
            set
            {
                _selectedTransport = value;
                OnPropertyChanged();

                if (_selectedTransport != null)
                {
                    Transport = new Transport
                    {
                        Id = _selectedTransport.Id,
                        TransportNumber = _selectedTransport.TransportNumber,
                        Name = _selectedTransport.Name,
                        TransportType = _selectedTransport.TransportType,
                        Capacity = _selectedTransport.Capacity
                    };

                    LoadTruckSegments();
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        // ComboBox Selected Type Filter
        private TransportFilterType? _selectedType = TransportFilterType.All;
        public TransportFilterType? SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                OnPropertyChanged();

                TransportView.Refresh();
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

                TransportView.Refresh();   // Обновление Вьев при каждом изменении данного свойства
            }
        }

        // ComboBox Filters Value
        private TransportFilter _selectedFilter;
        public TransportFilter SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();

                TransportView.Refresh();   // Обновление Вьев при каждом изменении данного свойства
            }
        }

        #endregion

        private bool CanAddTransport(object? parameter)
        {
            return Transport.TransportNumber > 0 &&
                   Transport.Name != string.Empty &&
                   Transport.Capacity > 0 &&
                   SelectedTransport == null &&
                   !_transportService.Exist(Transport.TransportNumber);
        }

        private void AddTransport(object? parameter)
        {
            Transport transport = new()
            {
                TransportNumber = Transport.TransportNumber,
                Name = Transport.Name,
                TransportType = Transport.TransportType,
                Capacity = Transport.Capacity
            };

            var createdTransport = _transportService.CreateTransport(transport);

            if (createdTransport != null) Transports.Add(createdTransport);

            // Clear
            Transport = new Transport();
            SelectedTransport = null;
        }

        private bool CanUpdateTransport(object? parameter)
        {
            return SelectedTransport != null;
        }

        private void UpdateTransport(object? parameter)
        {
            if (SelectedTransport == null) return;

            Transport transport = new()
            {
                TransportNumber = Transport.TransportNumber,
                Name = Transport.Name,
                TransportType = Transport.TransportType,
                Capacity = Transport.Capacity
            };

            var updatedTransport = _transportService.UpdateTransport(transport, SelectedTransport.Id);

            // UI
            if (updatedTransport != null)
            {
                int index = Transports.IndexOf(SelectedTransport);
                if (index >= 0) Transports[index] = updatedTransport;
                SelectedTransport = updatedTransport;
            }

            TransportView.Refresh();

            // Clear
            Transport = new Transport();
            SelectedTransport = null;
        }

        private bool CanRemoveTransport(object? parameter)
        {
            return SelectedTransport != null;
        }

        private void RemoveTransport(object? parameter)
        {
            if (SelectedTransport != null)
            {
                _transportService.DeleteTransport(SelectedTransport.Id);
                Transports.Remove(SelectedTransport);
            }

            // Clear
            Transport = new Transport();
            SelectedTransport = null;
        }

        private void ClearForm(object? parameter)
        {
            SelectedTransport = null;
            Transport = new Transport();
            SearchBoxText = string.Empty;
            SelectedFilter = TransportFilter.TransportNumber;
            TransportView.Refresh();
        }


        #region Filters

        private bool FilterTransportType(object? obj)
        {
            if (SelectedType == null || SelectedType == TransportFilterType.All) return true;

            var transport = obj as Transport;
            if (transport == null) return false;

            return SelectedType switch
            {
                TransportFilterType.Truck => transport.TransportType == TransportType.Truck,
                TransportFilterType.Ship => transport.TransportType == TransportType.Ship,
                _ => true
            };
        }

        private bool FilterTransport(object? obj)
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return true;

            var transport = obj as Transport;
            if (transport == null) return false;

            return SelectedFilter switch
            {
                TransportFilter.TransportNumber => transport.TransportNumber.ToString().Contains(SearchBoxText),
                TransportFilter.Name => transport.Name.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase),
                TransportFilter.TransportType => transport.TransportType.ToString().Contains(SearchBoxText),
                _ => true
            };
        }

        #endregion
    }
}
