namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Constants for simple reuse of similar error messages
/// on different entities.
/// </summary>
public class StandardErrorMessages
{
    public const string NameCouldNotBeFormattedAsAnId = "Name does not contain any characters that can be used to create an identifier (letters or numbers)";

    public const string StringMaxLength = "The length of '{PropertyName}' must be {MaxLength} characters or fewer";

    public static string NameIsNotUnique(string entityName)
    {
        return $"A {entityName} with this name already exists. The uniqueness check is based only on letters and numbers.";
    }
}
