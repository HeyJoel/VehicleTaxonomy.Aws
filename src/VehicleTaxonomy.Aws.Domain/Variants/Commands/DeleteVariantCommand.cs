namespace VehicleTaxonomy.Aws.Domain.Variants;

public class DeleteVariantCommand
{
    /// <summary>
    /// The unique id of the parent model that the variant belongs
    /// to e.g. "polo" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// Id of the variant to delete e.g. "3008-access-petrol-1-6" or
    /// "id3-city-battery-electric".
    /// </summary>
    public string VariantId { get; set; } = string.Empty;
}
