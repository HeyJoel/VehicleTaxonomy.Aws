namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Standard result type for entity add operations.
/// </summary>
public class AddEntityResult
{
    /// <summary>
    /// The string identifier that uniquely represents the
    /// created record e.g. "bmw" or "volkwagen-polo".
    /// </summary>
    public required string Id { get; set; }
}
