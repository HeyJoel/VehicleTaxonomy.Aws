using Amazon.Lambda.Annotations.APIGateway;

namespace VehicleTaxonomy.Aws.Api.Tests;

public static class IHttpResultHelpers
{
    public static string SerializeToText(this IHttpResult result)
    {
        using var stream = result.Serialize(new());
        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();

        return text;
    }
}
