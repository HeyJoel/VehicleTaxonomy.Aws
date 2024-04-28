using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using VehicleTaxonomy.Aws.Domain.Variants;

namespace VehicleTaxonomy.Aws.Api;

public class VariantsApi
{
    const string ROUTE_PREFIX = "/makes/{makeId}/models/{modelId}/variants";

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Get, ROUTE_PREFIX)]
    public async Task<IHttpResult> ListVariantsHandler(
        [FromServices] ListVariantsQueryHandler listVariantsQueryHandler,
        string makeId,
        string modelId,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ListVariantsQuery)}");

        var queryResponse = await listVariantsQueryHandler.ExecuteAsync(new()
        {
            ModelId = modelId
        });

        return ApiResponseHelper.ToHttpResult(queryResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Get, ROUTE_PREFIX + "/is-unique")]
    public async Task<IHttpResult> IsVariantUniqueHandler(
        [FromServices] IsVariantUniqueQueryHandler isVariantUniqueQueryHandler,
        string makeId,
        string modelId,
        [FromQuery] string name,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(IsVariantUniqueQuery)}");

        var queryResponse = await isVariantUniqueQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        });

        return ApiResponseHelper.ToHttpResult(queryResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, ROUTE_PREFIX)]
    public async Task<IHttpResult> AddVariantHandler(
        [FromServices] AddVariantCommandHandler addVariantCommandHandler,
        string makeId,
        string modelId,
        [FromBody] AddVariantCommand command,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(AddVariantCommand)}");

        command.MakeId = makeId;
        command.ModelId = modelId;
        var commandResponse = await addVariantCommandHandler.ExecuteAsync(command);

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Delete, ROUTE_PREFIX + "/{variantId}")]
    public async Task<IHttpResult> DeleteVariantHandler(
        [FromServices] DeleteVariantCommandHandler deleteVariantCommandHandler,
        string makeId,
        string modelId,
        string variantId,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(DeleteVariantCommand)}");

        var commandResponse = await deleteVariantCommandHandler.ExecuteAsync(new()
        {
            ModelId = modelId,
            VariantId = variantId
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }
}
