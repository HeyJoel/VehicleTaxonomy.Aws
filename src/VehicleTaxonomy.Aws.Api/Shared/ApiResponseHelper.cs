using Amazon.Lambda.Annotations.APIGateway;
using VehicleTaxonomy.Aws.Domain;

namespace VehicleTaxonomy.Aws.Api;

/// <summary>
/// Used to simplify and consistently apply the same formatting
/// across APIs.
/// </summary>
public static class ApiResponseHelper
{
    public static IHttpResult ToHttpResult<TResult>(QueryResponse<TResult> queryResponse)
    {
        var apiResponse = new ApiResponse()
        {
            IsValid = queryResponse.IsValid,
            Result = queryResponse.Result,
            ValidationErrors = queryResponse.ValidationErrors,
        };

        return ToHttpResult(queryResponse, apiResponse);
    }

    public static IHttpResult ToHttpResult<TResult>(CommandResponse<TResult> commandResponse)
    {
        var apiResponse = new ApiResponse()
        {
            IsValid = commandResponse.IsValid,
            Result = commandResponse.Result,
            ValidationErrors = commandResponse.ValidationErrors,
        };

        return ToHttpResult(commandResponse, apiResponse);
    }

    public static IHttpResult ToHttpResult(CommandResponse commandResponse)
    {
        var apiResponse = new ApiResponse()
        {
            IsValid = commandResponse.IsValid,
            ValidationErrors = commandResponse.ValidationErrors,
        };

        return ToHttpResult(commandResponse, apiResponse);
    }

    private static IHttpResult ToHttpResult(ICommandOrQueryResponse commandOrQueryResponse, ApiResponse apiResponse)
    {
        if (commandOrQueryResponse.IsValid)
        {
            return HttpResults.Ok(apiResponse);
        }
        else
        {
            return HttpResults.BadRequest(apiResponse);
        }
    }
}
