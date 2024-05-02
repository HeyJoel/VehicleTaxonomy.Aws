using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using VehicleTaxonomy.Aws.Domain.Models;

namespace VehicleTaxonomy.Aws.Api;

public class ModelsApi
{
    const string ROUTE_PREFIX = "/makes/{makeId}/models";

    [LambdaFunction(
        Policies = LambdaDefaultConfig.Policies,
        ResourceName = "ModelsApiList",
        MemorySize = 128
        )]
    [RestApi(LambdaHttpMethod.Get, ROUTE_PREFIX)]
    public async Task<IHttpResult> ListModelsHandler(
        [FromServices] ListModelsQueryHandler listModelsQueryHandler,
        string makeId,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ListModelsQuery)}");

        var queryResponse = await listModelsQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId
        });

        return ApiResponseHelper.ToHttpResult(queryResponse);
    }

    [LambdaFunction(
        Policies = LambdaDefaultConfig.Policies,
        ResourceName = "ModelsApiIsUnique",
        MemorySize = 128
        )]
    [RestApi(LambdaHttpMethod.Get, ROUTE_PREFIX + "/is-unique")]
    public async Task<IHttpResult> IsModelUniqueHandler(
        [FromServices] IsModelUniqueQueryHandler isModelUniqueQueryHandler,
        string makeId,
        [FromQuery] string name,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(IsModelUniqueQuery)}");

        var queryResponse = await isModelUniqueQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        return ApiResponseHelper.ToHttpResult(queryResponse);
    }

    [LambdaFunction(
        Policies = LambdaDefaultConfig.Policies,
        ResourceName = "ModelsApiAdd",
        MemorySize = 128
        )]
    [RestApi(LambdaHttpMethod.Post, ROUTE_PREFIX)]
    public async Task<IHttpResult> AddModelHandler(
        [FromServices] AddModelCommandHandler addModelCommandHandler,
        string makeId,
        [FromBody] AddModelCommand command,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(AddModelCommand)}");

        command.MakeId = makeId;
        var commandResponse = await addModelCommandHandler.ExecuteAsync(command);

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }

    [LambdaFunction(
        Policies = LambdaDefaultConfig.Policies,
        ResourceName = "ModelsApiDeleteById",
        MemorySize = 128
        )]
    [RestApi(LambdaHttpMethod.Delete, ROUTE_PREFIX + "/{modelId}")]
    public async Task<IHttpResult> DeleteModelHandler(
        [FromServices] DeleteModelCommandHandler deleteModelCommandHandler,
        string makeId,
        string modelId,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(DeleteModelCommand)}");

        var commandResponse = await deleteModelCommandHandler.ExecuteAsync(new()
        {
            ModelId = modelId,
            MakeId = makeId
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }
}
