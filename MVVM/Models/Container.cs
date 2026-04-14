using Catel.Data;
using PortBridgeShipping.Core.Collections.Enums;

namespace PortBridgeShipping.MVVM.Models
{
    public class Container : ValidatableModelBase
    {
        public int Id { get; set; }
        public int ContainerNumber { get; set; }
        public decimal ContainerWeight { get; set; }
        public ContainerType ContainerType { get; set; }

        // Status
        public int StatusId { get; set; }               // Foreign Key
        public Status Status { get; set; } = null!;     // Navigation Property

        // Route
        public int? RouteId { get; set; }                // Foreign Key
        public Route? Route { get; set; }                 // Navigation Property


        // Validation
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            // Number
            if (ContainerNumber <= 0)
                validationResults.Add(FieldValidationResult.CreateError(nameof(ContainerNumber), "Number must be > 0"));

            // Weight
            if (ContainerWeight <= 0)
                validationResults.Add(FieldValidationResult.CreateError(nameof(ContainerWeight), "Weight must be > 0"));

            // Type
            if (ContainerType == ContainerType.None)
                validationResults.Add(FieldValidationResult.CreateError(nameof(ContainerWeight), "Type cannot be none"));
            // Status
            if (Status == null)
                validationResults.Add(FieldValidationResult.CreateError(nameof(Status), "Status is required"));
        }
    }
}
