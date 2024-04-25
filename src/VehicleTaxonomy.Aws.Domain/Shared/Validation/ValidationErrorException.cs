namespace VehicleTaxonomy.Aws.Domain;

public class ValidationErrorException : Exception
{
    public ValidationErrorException()
        : base("An unknown validation error has occured.")
    {
    }

    public ValidationErrorException(string message)
        : base(message)
    {

    }

    public ValidationErrorException(string message, IReadOnlyCollection<ValidationError> errors)
        : base(message)
    {
        Errors = errors;
    }

    public ValidationErrorException(IReadOnlyCollection<ValidationError> errors)
        : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    public IReadOnlyCollection<ValidationError> Errors { get; } = [];

    private static string BuildErrorMessage(IEnumerable<ValidationError> errors)
    {
        var errorLines = errors.Select(e => $"{Environment.NewLine} - {e.Property}: {e.Message}");
        return "Unexpected validation failure: " + string.Join(string.Empty, errorLines);
    }
}
