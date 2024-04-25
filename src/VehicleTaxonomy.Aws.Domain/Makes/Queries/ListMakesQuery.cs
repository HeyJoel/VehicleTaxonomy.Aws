namespace VehicleTaxonomy.Aws.Domain.Makes;

public class ListMakesQuery
{
    /// <summary>
    /// Optionally filter by the make name, using a case-insensitive
    /// "starts-with" comparision.
    /// </summary>
    public string? Name { get; set; }
}
