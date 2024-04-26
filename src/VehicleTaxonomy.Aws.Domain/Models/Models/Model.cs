namespace VehicleTaxonomy.Aws.Domain.Models;

/// <summary>
/// A model of vehicle often with a distinct shape and chassis
/// e.g. for make "Volkswagen" modles could be "Polo", "ID.3",
/// "Golf Plus" etc. A Make has many models, and a model can have
/// many configurations or "variants".
/// </summary>
public class Model
{
    /// <summary>
    /// A unique url-safe string identifier for the model e.g. "polo",
    /// "305", "id-3" or "golf-plus".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the model e.g. "Polo", "305", "ID.3" or "Golf Plus".
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
