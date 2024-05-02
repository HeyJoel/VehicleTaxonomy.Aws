using System.Text.Json.Serialization;

namespace VehicleTaxonomy.Aws.Domain.Variants;

public class AddVariantCommand
{
    /// <summary>
    /// Required. The unique id of the parent make that the model associated
    /// with <see cref="ModelId"/> belongs
    /// to e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// The unique id of the parent model that the variant belongs
    /// to e.g. "polo" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the variant e.g. "POLO MATCH TDI Diesel 1.5" or
    /// "3008 ACCESS Petrol 1.6". This will be converted to an id that
    /// is passed back in the handler result.
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
