using FluentValidation.Results;

namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Standardized response for command execution. If there is a validation
/// error this will be detailed in the response, otherwise <see cref="IsValid"/>
/// will be <see langword="true"/> and it can be assumed command execution
/// was successful. Any unexpected errors during handler execution will throw
/// an exception.
/// </summary>
public class CommandResponse : ICommandOrQueryResponse
{
    private static readonly CommandResponse _successResult = new();

    public CommandResponse()
    {
        IsValid = true;
        ValidationErrors = [];
    }

    public CommandResponse(IReadOnlyCollection<ValidationError> errors)
    {
        ValidationErrors = errors;
        IsValid = errors.Count == 0;
    }

    /// <summary>
    /// <see langword="true"/> if the command is valid and the handler
    /// executed successfully; otherwise <see langword="false"/>.
    /// </summary>
    public bool IsValid { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }

    public static CommandResponse Success()
    {
        return _successResult;
    }

    public static CommandResponse Error(string message, string? property)
    {
        return Error(new ValidationError(message, property));
    }

    public static CommandResponse Error(params ValidationError[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(errors), $"The {nameof(errors)} collection should contain at least one error.");
        }
        return new CommandResponse(errors);
    }

    public static CommandResponse Error(ValidationResult fluentValidationResult)
    {
        return Error(ValidationError.Map(fluentValidationResult));
    }
}

