using Catel.Data;
using PortBridgeShipping.Core.Collections.Enums;

namespace PortBridgeShipping.MVVM.Models
{
    public class RouteSegment : ValidatableModelBase
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string From { get; set; } = string.Empty;    // Initial Country
        public string To { get; set; } = string.Empty;      // Last Country

        // Transports
        public int TransportId { get; set; }                // Foreing Key
        public Transport Transport { get; set; } = null!;   // Navigation Property

        // Routes
        public int RouteId { get; set; }            // Foreing Key
        public Route Route { get; set; } = null!;   // Navigation Property


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
