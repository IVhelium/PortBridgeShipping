using Catel.Data;

namespace PortBridgeShipping.MVVM.Models
{
    public class Route : ValidatableModelBase
    {
        public int Id { get; set; }     // Auto-Increment
        public string Name { get; set; } = string.Empty;

        // RouteSegment
        public List<RouteSegment> Segments { get; set; } = new List<RouteSegment>();


        // Validation
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(Name))
                validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Route name is required"));

            if (Segments == null || Segments.Count == 0)
                validationResults.Add(FieldValidationResult.CreateError(nameof(Segments), "Route must have at least one segment"));
        }


        // Design output
        public string Display
        {
            get
            {
                if (Segments == null || Segments.Count == 0) return Name;

                var order = Segments.OrderBy(s => s.Order).ToList();    // Сортировка сегментов по порядку

                // From -> To
                var segment = order.Select(s => s.From).ToList();
                segment.Add(order.Last().To);                           // Возвращает последний элемент

                return string.Join(" -> ", segment);
            }
        }

        public override string ToString()
        {
            return Display;
        }
    }
}
