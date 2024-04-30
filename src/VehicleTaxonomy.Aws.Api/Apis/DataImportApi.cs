using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using VehicleTaxonomy.Aws.Domain;
using VehicleTaxonomy.Aws.Domain.DataImport;

namespace VehicleTaxonomy.Aws.Api;

public class DataImportApi
{
    const string ROUTE_PREFIX = "/data-import";

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, ROUTE_PREFIX + "/taxonomy")]
    public async Task<IHttpResult> TaxonomyImport(
        [FromServices] ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler,
        [FromBody] Stream content,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ImportTaxonomyFromCsvCommand)}");
        var fileSource = new StreamFileSource(context.AwsRequestId, () => content);

        var commandResponse = await importTaxonomyFromCsvCommandHandler.ExecuteAsync(new()
        {
            File = fileSource,
            ImportMode = DataImportMode.Run
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, ROUTE_PREFIX + "/taxonomy/validate")]
    public async Task<IHttpResult> TaxonomyImportValidate(
        [FromServices] ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler,
        [FromBody] Stream content,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ImportTaxonomyFromCsvCommand)}");
        var fileSource = new StreamFileSource(context.AwsRequestId, () => content);

        var commandResponse = await importTaxonomyFromCsvCommandHandler.ExecuteAsync(new()
        {
            File = fileSource,
            ImportMode = DataImportMode.Validate
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }
}
