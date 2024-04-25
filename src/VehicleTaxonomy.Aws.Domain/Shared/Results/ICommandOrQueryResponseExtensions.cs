namespace VehicleTaxonomy.Aws.Domain;

public static class ICommandOrQueryResponseExtensions
{
    public static void ThrowIfInvalid<TResponse>(this TResponse response)
        where TResponse : ICommandOrQueryResponse
    {
        if (!response.IsValid)
        {
            throw new ValidationErrorException(response.ValidationErrors);
        }
    }
}
