namespace VehicleTaxonomy.Aws.Domain.Models;

public class IsModelUniqueQuery
{
    /// <summary>
    /// Required. The unique id of the parent make that the model belongs
    /// to e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// Required. The name of the model to check for uniqueness e.g. "Polo",
    /// "305" or "Golf Plus". The uniqueness check is based on the
    /// normalized version of the name which only includes letters (a-z),
    /// numbers and dashes, and is case-insensitive.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
