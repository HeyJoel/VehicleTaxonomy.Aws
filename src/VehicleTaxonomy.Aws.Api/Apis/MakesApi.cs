using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using VehicleTaxonomy.Aws.Domain.Makes;

namespace VehicleTaxonomy.Aws.Api;

public class MakesApi
{
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Get, "/makes")]
    public async Task<IHttpResult> ListMakesHandler(
        [FromServices] ListMakesQueryHandler listMakesQueryHandler,
        [FromQuery] string? name,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(ListMakesQuery)}");

        var queryResponse = await listMakesQueryHandler.ExecuteAsync(new()
        {
            Name = name
        });

        return ApiResponseHelper.ToHttpResult(queryResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Get, "/makes/is-unique")]
    public async Task<IHttpResult> IsMakeUniqueHandler(
        [FromServices] IsMakeUniqueQueryHandler isMakeUniqueQueryHandler,
        [FromQuery] string name,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(IsMakeUniqueQuery)}");

        var queryResponse = await isMakeUniqueQueryHandler.ExecuteAsync(new()
        {
            Name = name
        });

        return ApiResponseHelper.ToHttpResult(queryResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/makes")]
    public async Task<IHttpResult> AddMakeHandler(
        [FromServices] AddMakeCommandHandler addMakeCommandHandler,
        [FromBody] AddMakeCommand command,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(AddMakeCommand)}");

        var commandResponse = await addMakeCommandHandler.ExecuteAsync(command);

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Delete, "/makes/{id}")]
    public async Task<IHttpResult> DeleteMakeHandler(
        [FromServices] DeleteMakeCommandHandler deleteMakeCommandHandler,
        string id,
        ILambdaContext context
        )
    {
        context.Logger.LogInformation($"Executing {nameof(DeleteMakeCommand)}");

        var commandResponse = await deleteMakeCommandHandler.ExecuteAsync(new()
        {
            MakeId = id
        });

        return ApiResponseHelper.ToHttpResult(commandResponse);
    }
}
