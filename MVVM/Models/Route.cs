using Catel.Data;

namespace PortBridgeShipping.MVVM.Models
{
    public class Route : ValidatableModelBase
    {
        public int Id { get; set; }     // Auto-Increment
        public string Name { get; set; } = string.Empty;

        public ICollection<RouteSegment> Segments { get; set; } = new List<RouteSegment>();   // RouteSegment
        public ICollection<Container> Containers { get; set; } = new List<Container>();       // Containers


        // Validation
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(Name))
                validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Route name is required"));

            if (Segments == null || Segments.Count == 0)
                validationResults.Add(FieldValidationResult.CreateError(nameof(Segments), "Route must have at least one segment"));


            var sortedSegments = Segments?.OrderBy(seg => seg.Order).ToList();

            if (sortedSegments != null && sortedSegments.Count > 0)
            {
                for (int i = 1; i < sortedSegments?.Count; i++)
                {
                    var currentSeg = sortedSegments[i];
                    var previousSeg = sortedSegments[i - 1];

                    if (currentSeg.From != previousSeg.To)
                        validationResults.Add(FieldValidationResult.CreateError(nameof(Segments), $"The “From” field in the new segment must match the previous one “{previousSeg.To}”"));
                }
            }
        }
    }
}
