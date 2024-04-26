namespace VehicleTaxonomy.Aws.Domain.Makes;

public class IsMakeUniqueQuery
{
    /// <summary>
    /// Required. The name of the make to check for uniqueness e.g. "Volkswagen",
    /// "MG" or "Mercedes Benz". The uniqueness check is based on the
    /// normalized version of the name which only includes letters (a-z),
    /// numbers and dashes, and is case-insensitive.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
