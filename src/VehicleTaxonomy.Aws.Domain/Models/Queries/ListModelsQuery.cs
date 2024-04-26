namespace VehicleTaxonomy.Aws.Domain.Models;

public class ListModelsQuery
{
    /// <summary>
    /// Required. The unique id of the parent make to filter models by
    /// e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;
}
