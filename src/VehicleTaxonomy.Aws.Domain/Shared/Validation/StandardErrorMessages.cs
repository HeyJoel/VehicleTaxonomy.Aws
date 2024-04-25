namespace VehicleTaxonomy.Aws.Domain.Shared.Validation;

/// <summary>
/// Constants for simple reuse of similar error messages
/// on different entities.
/// </summary>
public class StandardErrorMessages
{
    public const string NameCouldNotBeFormattedAsAnId = "Name does not contain any characters that can be used to create an identifier (letters or numbers)";

    public static string NameIsNotUnique(string entityName)
    {
        return $"A {entityName} with this name already exists. The uniqueness check is based only on letters and numbers.";
    }
}
