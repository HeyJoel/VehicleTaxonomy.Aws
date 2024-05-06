using System.Text.Json.Serialization;

namespace VehicleTaxonomy.Aws.Domain.Variants;

/// <summary>
/// AKA derivative or configuration. A specific configuration of
/// a vehicle model combining attributes such as the "trim" level,
/// fuel type and engine size e.g. Volkswagen Polo variants
/// include "POLO MATCH TDI 1.5l Diesel", "POLO S 1.2l Petrol" and
/// "POLO S 75 AUTO 1.4l Petrol".
/// </summary>
public class Variant
{
    /// <summary>
    /// A unique url-safe string identifier for the variant e.g.
    /// "polo-match-tdi-1-5l-deisel", "3008-access-1-6l-petrol" or
    /// "id3-city-battery-electric".
    /// </summary>
    public string VariantId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the make e.g. "POLO MATCH TDI 1.5l Diesel", "3008 ACCESS Petrol 1.6",
    /// "ID3 CITY Battery electric" etc.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of fuel the vehicle uses e.g. <see cref="FuelCategory.Petrol"/>,
    /// <see cref="FuelCategory.ElectricHybridDiesel"/> or <see cref="FuelCategory.Other"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<FuelCategory>))]
    public FuelCategory? FuelCategory { get; set; }

    /// <summary>
    /// The size of the engine in Cubic Centimetres (CC) e.g. 125, 600, 3800.
    /// This is more customer-friendly "Badge Size" rather than the exact value,
    /// which has been rounded to a sensible bracket e.g. 49 is rounded to 50,
    /// or 2335 is rounded to 2400.
    /// </summary>
    public int? EngineSizeInCC { get; set; }
}
