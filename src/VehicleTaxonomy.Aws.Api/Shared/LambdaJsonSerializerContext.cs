using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using VehicleTaxonomy.Aws.Domain.Makes;
using VehicleTaxonomy.Aws.Domain.Models;
using VehicleTaxonomy.Aws.Infrastructure.DataImport;

namespace VehicleTaxonomy.Aws.Api;

/// <summary>
/// This class is used to register the input event and return type for the FunctionHandler method with the System.Text.Json source generator.
/// There must be a JsonSerializable attribute for each type used as the input and return type or a runtime error will occur 
/// from the JSON serializer unable to find the serialization information for unknown types.
/// 
/// Request and response types used with Annotations library must also have a JsonSerializable for the type. The Annotations library will use the same
/// source generator serializer to use non-reflection based serialization. For example parameters with the [FromBody] or types returned using 
/// the HttpResults utility class.
/// </summary>
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(Make))]
[JsonSerializable(typeof(AddMakeCommand))]
[JsonSerializable(typeof(Model))]
[JsonSerializable(typeof(AddModelCommand))]
[JsonSerializable(typeof(DataImportJobResult))]
[JsonSerializable(typeof(DataImportJobStatus))]
[JsonSourceGenerationOptions(
    UseStringEnumConverter = true, // NB: this doesn't seem to work unless you annotate the property itself, see DataImportJobResult.Status.
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class LambdaJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}
