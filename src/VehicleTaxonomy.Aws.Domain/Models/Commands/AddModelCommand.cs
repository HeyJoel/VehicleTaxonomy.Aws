namespace VehicleTaxonomy.Aws.Domain.Models;

public class AddModelCommand
{
    /// <summary>
    /// The unique id of the parent make that the model belongs
    /// to e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the model e.g. "Polo", "305" or "Golf Plus".
    /// This will be converted to an id that is passed back in the handler
    /// result.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
