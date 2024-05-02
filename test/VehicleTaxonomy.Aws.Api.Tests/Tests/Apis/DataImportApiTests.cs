using System.Text;
using VehicleTaxonomy.Aws.Domain.DataImport;

namespace VehicleTaxonomy.Aws.Api.Tests;

/// <summary>
/// Basic endpoint tests. Logic is tested in the domain test project.
/// </summary>
[Collection(nameof(ServiceDependentFixture))]
public class DataImportApiTests
{
    private readonly ServiceDependentFixture _serviceDependentFixture;

    public DataImportApiTests(
        ServiceDependentFixture serviceDependentFixture
        )
    {
        _serviceDependentFixture = serviceDependentFixture;
    }

    [Fact]
    public async Task TaxonomyImport_CanImport()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var importTaxonomyFromCsvCommandHandler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();

        var context = new TestLambdaContext()
        {
            AwsRequestId = Guid.NewGuid().ToString("N")
        };
        var dataImportApi = new DataImportApi();

        var csv = Encoding.UTF8.GetBytes(TEST_CSV);
        using var stream = new MemoryStream(csv);

        var result = await dataImportApi.TaxonomyImport(
            importTaxonomyFromCsvCommandHandler,
            TEST_CSV,
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:{\u0022NumSuccess\u0022:1,\u0022NumSkipped\u0022:0,\u0022NumInvalid\u0022:0,\u0022Status\u0022:\u0022Finished\u0022,\u0022SkippedReasons\u0022:{},\u0022ValidationErrors\u0022:{}},\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task TaxonomyImportValidate_CanValidate()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var importTaxonomyFromCsvCommandHandler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();

        var context = new TestLambdaContext()
        {
            AwsRequestId = Guid.NewGuid().ToString("N")
        };
        var dataImportApi = new DataImportApi();

        var csv = Encoding.UTF8.GetBytes(TEST_CSV);
        using var stream = new MemoryStream(csv);

        var result = await dataImportApi.TaxonomyImportValidate(
            importTaxonomyFromCsvCommandHandler,
            TEST_CSV,
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:{\u0022NumSuccess\u0022:1,\u0022NumSkipped\u0022:0,\u0022NumInvalid\u0022:0,\u0022Status\u0022:\u0022Finished\u0022,\u0022SkippedReasons\u0022:{},\u0022ValidationErrors\u0022:{}},\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    const string TEST_CSV = """
        BodyType,Make,GenModel,Model,Fuel,EngineSizeSimple,EngineSizeDesc
        Cars,ABARTH,ABARTH 124,124 GT MULTIAIR,Petrol,1400,1301cc to 1400cc
        """;
}
