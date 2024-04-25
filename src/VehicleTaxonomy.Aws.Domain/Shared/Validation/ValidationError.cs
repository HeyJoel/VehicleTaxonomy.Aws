using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Represents a single error when validating an object.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Represents a single error when validating an object.
    /// </summary>
    public ValidationError()
    {
    }

    /// <summary>
    /// Represents a single error when validating an object.
    /// </summary>
    /// <param name="message">Client-friendly text describing the error.</param>
    /// <param name="property">Optional property that the error message applies to.</param>
    [SetsRequiredMembers]
    public ValidationError(string message, string? property = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Message = message;
        Property = property;
    }

    /// <summary>
    /// Optional property that the error message applies to.
    /// </summary>
    public string? Property { get; set; } = string.Empty;

    /// <summary>
    /// Client-friendly text describing the error.
    /// </summary>
    public required string Message { get; set; }

    internal static ValidationError[] Map(ValidationResult fluentValidationResult)
    {
        if (fluentValidationResult.IsValid)
        {
            throw new InvalidOperationException($"{nameof(ValidationError)}.{nameof(Map)} should not be called with a valid result.");
        }
        return Map(fluentValidationResult.Errors);
    }

    internal static ValidationError[] Map(IReadOnlyCollection<ValidationFailure> errors)
    {
        var mappedErrors = errors
            .Select(e => new ValidationError()
            {
                Message = e.ErrorMessage,
                Property = e.PropertyName == string.Empty ? null : e.PropertyName
            })
            .ToArray();

        return mappedErrors;
    }
}
