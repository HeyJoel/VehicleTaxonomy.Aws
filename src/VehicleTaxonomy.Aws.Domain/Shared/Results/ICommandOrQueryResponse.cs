namespace VehicleTaxonomy.Aws.Domain;

/// <summary>
/// Standardized response for query or command execution. If there is a validation
/// error this will be detailed in the response, otherwise <see cref="IsValid"/>
/// will be <see langword="true"/> and it can be assumed execution was successful.
/// Any unexpected errors during handler execution will throw an exception.
/// </summary>
public interface ICommandOrQueryResponse
{
    /// <summary>
    /// <see langword="true"/> if the command or query is valid
    /// and the handler executed successfully; otherwise <see langword="false"/>.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// If <see cref="IsValid"/> is <see langword="false"/>, this collection will
    /// include at least one error containing a user-display-friendly message. This
    /// will only every contain validation messages, any unexpected errors during handler
    /// execution will throw an exception.
    /// </summary>
    IReadOnlyCollection<ValidationError> ValidationErrors { get; }
}
