namespace VehicleTaxonomy.Aws.Infrastructure.Db;

public class VariantData
{
    /// <summary>
    /// The type of fuel the vehicle uses. Maps
    /// to the FuelCategory enum in the domain.
    /// </summary>
    public string? FuelCategory { get; set; }

    /// <summary>
    /// The size of the engine in Cubic Centimetres (CC) e.g. 125, 600, 3800.
    /// This is more customer-friendly "Badge Size" rather than the exact value,
    /// which has been rounded to a sensible bracket e.g. 49 is rounded to 50,
    /// or 2335 is rounded to 2400.
    /// </summary>
    public int? EngineSizeInCC { get; set; }
}
