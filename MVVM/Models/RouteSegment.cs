using System.Collections.ObjectModel;
using Catel.Data;

namespace PortBridgeShipping.MVVM.Models
{
    public class RouteSegment : ValidatableModelBase
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string From { get; set; } = string.Empty;    // Initial Country
        public string To { get; set; } = string.Empty;      // Last Country

        // Routes
        public int RouteId { get; set; }            // Foreing Key
        public Route Route { get; set; } = null!;   // Navigation Property

        // Transports
        public ObservableCollection<RouteSegmentTransport> SegmentTransports { get; set; } = new ObservableCollection<RouteSegmentTransport>();


        // Validation
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(From))
                validationResults.Add(FieldValidationResult.CreateError(nameof(From), "From is required"));

            if (string.IsNullOrWhiteSpace(To))
                validationResults.Add(FieldValidationResult.CreateError(nameof(To), "To is required"));
        }
    }
}
