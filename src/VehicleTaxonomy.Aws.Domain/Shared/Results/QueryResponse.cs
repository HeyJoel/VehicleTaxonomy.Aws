using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Standardized response for query execution where the result is not
/// nullable. If there is a validation error this will be detailed in the
/// response, otherwise <see cref="IsValid"/> will be <see langword="true"/>
/// and it can be assumed query execution was successful. Any unexpected errors
/// during handler execution will throw an exception.
/// </summary>
public class QueryResponse<TResult> : ICommandOrQueryResponse
{
    public QueryResponse(TResult result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), $"A successful {nameof(result)} should always have a {nameof(result)} value.");
        }

        IsValid = true;
        ValidationErrors = [];
        Result = result;
    }

    public QueryResponse(IReadOnlyCollection<ValidationError> errors)
    {
        ValidationErrors = errors;
        IsValid = errors.Count == 0;
    }

    /// <summary>
    /// <see langword="true"/> if the query is valid and the handler
    /// executed successfully; otherwise <see langword="false"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool IsValid { get; }

    /// <summary>
    /// If execution was successful then this will contain the query result
    /// data. If execution was not successful then this will be <see langword="null"/>.
    /// </summary>
    public TResult? Result { get; set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }

    public static QueryResponse<TResult> Success(TResult result)
    {
        return new QueryResponse<TResult>(result);
    }

    public static QueryResponse<TResult> Error(string message, string? property)
    {
        return Error(new ValidationError(message, property));
    }

    public static QueryResponse<TResult> Error(params ValidationError[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(errors), $"The {nameof(errors)} collection should contain at least one error.");
        }
        return new QueryResponse<TResult>(errors);
    }

    public static QueryResponse<TResult> Error(ValidationResult fluentValidationResult)
    {
        return Error(ValidationError.Map(fluentValidationResult));
    }
}

