using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Standardized response for command execution where custom data needs to be
/// passed back to the callee e.g. the id of a newly created record. If there
/// is a validation eror this will be detailed in the response, otherwise <see cref="IsValid"/>
/// will be <see langword="true"/> and it can be assumed command execution
/// was successful. Any unexpected errors during handler execution will throw
/// an exception.
/// </summary>
public class CommandResponse<TResult> : ICommandOrQueryResponse
{
    public CommandResponse(TResult result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), $"A successful {nameof(result)} should always have a {nameof(result)} value.");
        }

        IsValid = true;
        ValidationErrors = [];
        Result = result;
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
    [MemberNotNullWhen(true, nameof(Result))]
    public bool IsValid { get; }

    /// <summary>
    /// If execution was successful then this will contain any custom data
    /// that needs to be passed back to the callee e.g. e.g. the id of a newly
    /// created record.
    /// </summary>
    public TResult? Result { get; set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }

    public static CommandResponse<TResult> Success(TResult result)
    {
        return new CommandResponse<TResult>(result);
    }

    public static CommandResponse<TResult> Error(string message, string? property)
    {
        return Error(new ValidationError(message, property));
    }

    public static CommandResponse<TResult> Error(params ValidationError[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(errors), $"The {nameof(errors)} collection should contain at least one error.");
        }
        return new CommandResponse<TResult>(errors);
    }

    public static CommandResponse<TResult> Error(ValidationResult fluentValidationResult)
    {
        return Error(ValidationError.Map(fluentValidationResult));
    }
}
