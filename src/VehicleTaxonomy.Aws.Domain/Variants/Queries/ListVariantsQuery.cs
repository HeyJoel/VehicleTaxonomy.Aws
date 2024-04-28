namespace VehicleTaxonomy.Aws.Domain.Variants;

public class ListVariantsQuery
{
    /// <summary>
    /// The unique id of the parent model that the variant belongs
    /// to e.g. "polo" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;
}