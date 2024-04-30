namespace VehicleTaxonomy.Aws.Domain.Variants;

public class ListVariantsQuery
{
    /// <summary>
    /// Required. The unique id of the parent make to filter variants by
    /// e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// The unique id of the parent model that the variant belongs
    /// to e.g. "polo" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;
}
