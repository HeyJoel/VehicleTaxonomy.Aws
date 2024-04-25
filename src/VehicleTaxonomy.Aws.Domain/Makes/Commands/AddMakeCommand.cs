namespace VehicleTaxonomy.Aws.Domain.Makes;

public class AddMakeCommand
{
    /// <summary>
    /// The name of the make e.g. "Volkswagen", "MG" or "Mercedes Benz".
    /// This will be converted to an id that is passed back in the handler
    /// result.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
