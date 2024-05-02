using System.Text;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using VehicleTaxonomy.Aws.Domain;
using VehicleTaxonomy.Aws.Domain.DataImport;

namespace VehicleTaxonomy.Aws.Api;

public class DataImportApi
{
    const string ROUTE_PREFIX = "/data-import";

    [LambdaFunction(
        Policies = LambdaDefaultConfig.Policies,
        ResourceName = "DataImportApiTaxonomyImport"
        )]
    [RestApi(LambdaHttpMethod.Post, ROUTE_PREFIX + "/taxonomy")]
    public async Task<IHttpResult> TaxonomyImport(
        [FromServices] ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler,
        [FromBody] string csvFile,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ImportTaxonomyFromCsvCommand)}");

        var byteArray = Encoding.UTF8.GetBytes(csvFile);
        using var inputStream = new MemoryStream(byteArray);
        var fileSource = new StreamFileSource(context.AwsRequestId, () => inputStream);

        var commandResponse = await importTaxonomyFromCsvCommandHandler.ExecuteAsync(new()
        {
            File = fileSource,
            ImportMode = DataImportMode.Run
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }

    [LambdaFunction(
        Policies = LambdaDefaultConfig.Policies,
        ResourceName = "DataImportApiTaxonomyValidate"
        )]
    [RestApi(LambdaHttpMethod.Post, ROUTE_PREFIX + "/taxonomy/validate")]
    public async Task<IHttpResult> TaxonomyImportValidate(
        [FromServices] ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler,
        [FromBody] string csvFile,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ImportTaxonomyFromCsvCommand)}");
        context.Logger.LogInformation($"Found file of length " + csvFile.Length);
        context.Logger.LogInformation($"Recived: '{csvFile.Substring(0, 100)}'");

        var byteArray = Encoding.UTF8.GetBytes(csvFile);
        using var inputStream = new MemoryStream(byteArray);
        var fileSource = new StreamFileSource(context.AwsRequestId, () => inputStream);

        var commandResponse = await importTaxonomyFromCsvCommandHandler.ExecuteAsync(new()
        {
            File = fileSource,
            ImportMode = DataImportMode.Validate
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }
}
