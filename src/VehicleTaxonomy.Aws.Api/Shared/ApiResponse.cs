using VehicleTaxonomy.Aws.Domain;

namespace VehicleTaxonomy.Aws.Api;

/// <summary>
/// A simple AOT-serializable wrapper for all API responses for consistency.
/// </summary>
public class ApiResponse
{
    public bool IsValid { get; set; }

    public object? Result { get; set; }

    public IReadOnlyCollection<ValidationError> ValidationErrors { get; set; } = [];
}
