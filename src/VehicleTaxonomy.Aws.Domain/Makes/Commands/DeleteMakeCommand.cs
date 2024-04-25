namespace VehicleTaxonomy.Aws.Domain.Makes;

public class DeleteMakeCommand
{
    /// <summary>
    /// Id of the make to delete e.g. "volkswagen", "mg" or "mercedes-benz".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;
}
