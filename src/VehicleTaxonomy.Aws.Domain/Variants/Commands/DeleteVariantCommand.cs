namespace VehicleTaxonomy.Aws.Domain.Variants;

public class DeleteVariantCommand
{
    /// <summary>
    /// The unique id of the parent make that the model belongs
    /// to e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// The unique id of the parent model that the variant belongs
    /// to e.g. "polo" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// Id of the variant to delete e.g. "3008-access-1-6l-petrol" or
    /// "id3-city-battery-electric".
    /// </summary>
    public string VariantId { get; set; } = string.Empty;
}
