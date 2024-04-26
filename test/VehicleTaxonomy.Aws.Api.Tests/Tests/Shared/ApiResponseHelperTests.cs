namespace VehicleTaxonomy.Aws.Api.Tests.Shared;

/// <summary>
/// API tests already test the OK condition. Here we just need to cover
/// the mapping of errors.
/// </summary>
public class ApiResponseHelperTests
{
    [Fact]
    public void Respond_WithInvalidQueryResponse_Maps()
    {
        var response = QueryResponse<string?>.Error("Query error", "MyQueryProp");
        var result = ApiResponseHelper.ToHttpResult(response);

        var output = result.SerializeToText();
        InlineSnapshot.Validate(output, """
            {"statusCode":400,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:false,\u0022Result\u0022:null,\u0022ValidationErrors\u0022:[{\u0022Property\u0022:\u0022MyQueryProp\u0022,\u0022Message\u0022:\u0022Query error\u0022}]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public void Respond_WithInvalidCommandResponse_Maps()
    {
        var response = CommandResponse.Error("Command error", "MyCommandProp");
        var result = ApiResponseHelper.ToHttpResult(response);

        var output = result.SerializeToText();
        InlineSnapshot.Validate(output, """
            {"statusCode":400,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:false,\u0022Result\u0022:null,\u0022ValidationErrors\u0022:[{\u0022Property\u0022:\u0022MyCommandProp\u0022,\u0022Message\u0022:\u0022Command error\u0022}]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public void Respond_WithInvalidCommandWithResultResponse_Maps()
    {
        var response = CommandResponse<string?>.Error("CommandT error", "MyCommandTProp");
        var result = ApiResponseHelper.ToHttpResult(response);

        var output = result.SerializeToText();
        InlineSnapshot.Validate(output, """
            {"statusCode":400,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:false,\u0022Result\u0022:null,\u0022ValidationErrors\u0022:[{\u0022Property\u0022:\u0022MyCommandTProp\u0022,\u0022Message\u0022:\u0022CommandT error\u0022}]}","isBase64Encoded":false}
            """);
    }
}
