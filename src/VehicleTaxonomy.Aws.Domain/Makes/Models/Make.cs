namespace VehicleTaxonomy.Aws.Domain.Makes;

/// <summary>
/// AKA Brand or Marque. The retail name of the company that
/// produces a range of vehicle models. Note that a parent company
/// may own a range of makes e.g. "Volkswagen", "Audi" and "Porche" are
/// all makes owned by the "Volkswagen Group" organisation.
/// </summary>
public class Make
{
    /// <summary>
    /// A unique url-safe string identifier for the make
    /// e.g. "volkswagen", "mg" or "mercedes-benz"
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the make e.g. "Volkswagen", "MG" or "Mercedes Benz"
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
