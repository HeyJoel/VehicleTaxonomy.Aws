namespace VehicleTaxonomy.Aws.Domain.Variants;

public class IsVariantUniqueQuery
{
    /// <summary>
    /// Required. The unique id of the parent make that the model
    /// assopciated with <see cref="ModelId"/> belongs to e.g.
    /// "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// Required. The unique id of the parent model that the variant belongs
    /// to e.g. "polo" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// Required. The name of the variant to check for uniqueness e.g.
    /// "POLO MATCH TDI 1.5l Diesel" or "3008 ACCESS Petrol 1.6". The uniqueness
    /// check is based on the normalized version of the name which only includes
    /// letters (a-z), numbers and dashes, and is case-insensitive.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
