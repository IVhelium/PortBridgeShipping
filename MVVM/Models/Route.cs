using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Catel.Data;

namespace PortBridgeShipping.MVVM.Models
{
    public class Route : ValidatableModelBase
    {
        public int Id { get; set; }     // Auto-Increment
        public string Name { get; set; } = string.Empty;

        public ObservableCollection<RouteSegment> Segments { get; set; } = new();   // RouteSegment
        public ObservableCollection<Container> Containers { get; set; } = new();    // Containers


        // Validation
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(Name))
                validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Route name is required"));

            if (Segments == null || Segments.Count == 0)
                validationResults.Add(FieldValidationResult.CreateError(nameof(Segments), "Route must have at least one segment"));


            var sortedSegments = Segments?.OrderBy(seg => seg.Order).ToList();

            for (int i = 1; i < sortedSegments?.Count; i++)
            {
                var currentSeg = sortedSegments[i];
                var previousSeg = sortedSegments[i - 1];

                if (currentSeg.From != previousSeg.To)
                    validationResults.Add(FieldValidationResult.CreateError(nameof(Segments), $"The “From” field in the new segment must match the previous one “{previousSeg.To}”"));
            }
        }


        // Design output
        [NotMapped]
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
