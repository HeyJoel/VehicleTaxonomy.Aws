namespace VehicleTaxonomy.Aws.Domain.Variants.Models;

/// <summary>
/// A simplified range of fuel types.
/// </summary>
public enum FuelCategory
{
    /// <summary>
    /// Anything that does not match the other values. This could
    /// included hyrogen or other alternative fuels.
    /// </summary>
    Other,

    /// <summary>
    /// Fully electric via battery or fuel cell.
    /// </summary>
    Electric,

    /// <summary>
    /// An electric/petrol hybrid. Includes plug-in and non-plug-in hybrids.
    /// </summary>
    ElectricHybridPetrol,

    /// <summary>
    /// An electric/diesel hybrid. Includes plug-in and non-plug-in hybrids.
    /// </summary>
    ElectricHybridDiesel,

    /// <summary>
    /// Diesel ICE (internal combustion engine).
    /// </summary>
    Diesel,

    /// <summary>
    /// Petrol ICE (internal combustion engine).
    /// </summary>
    Petrol
}
