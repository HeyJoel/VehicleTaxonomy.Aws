using VehicleTaxonomy.Aws.Domain.Makes;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Api.Tests;

/// <summary>
/// Basic endpoint tests. Logic is tested in the domain test project.
/// </summary>
[Collection(nameof(ServiceDependentFixture))]
public class MakeApiTests
{
    private const string UNIQUE_DATA = nameof(MakeApiTests);

    private readonly ServiceDependentFixture _serviceDependentFixture;

    public MakeApiTests(
        ServiceDependentFixture serviceDependentFixture
        )
    {
        _serviceDependentFixture = serviceDependentFixture;
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var startup = new Startup();
        startup.ConfigureServices(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ListMakes_CanQuery()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var listMakesQueryHandler = scope.ServiceProvider.GetRequiredService<ListMakesQueryHandler>();

        var context = new TestLambdaContext();
        var makesApi = new MakesApi();
        var result = await makesApi.ListMakesHandler(
            listMakesQueryHandler,
            "Volks",
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:[{\u0022MakeId\u0022:\u0022volkswagen\u0022,\u0022Name\u0022:\u0022Volkswagen\u0022}],\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task IsMakeUnique_CanQuery()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var isMakeUniqueQueryHandler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();

        var context = new TestLambdaContext();
        var makesApi = new MakesApi();
        var result = await makesApi.IsMakeUniqueHandler(
            isMakeUniqueQueryHandler,
            nameof(IsMakeUnique_CanQuery),
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:true,\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task AddMake_CanAdd()
    {
        var name = UNIQUE_DATA + nameof(AddMake_CanAdd);
        var id = EntityIdFormatter.Format(name);

        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var addMakeCommandHandler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleTaxonomyRepository>();

        var context = new TestLambdaContext();
        var makesApi = new MakesApi();
        var command = new AddMakeCommand()
        {
            Name = name
        };

        var result = await makesApi.AddMakeHandler(
            addMakeCommandHandler,
            command,
            context
            );

        var dbRecord = await repository.GetByIdAsync(VehicleTaxonomyEntityType.Make, id, null);
        var output = result.SerializeToText();

        Assert.NotNull(dbRecord);
        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:{\u0022Id\u0022:\u0022makeapitestsaddmake-canadd\u0022},\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task AddMake_CanDelete()
    {
        var name = UNIQUE_DATA + nameof(AddMake_CanDelete);
        var id = EntityIdFormatter.Format(name);

        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var deleteMakeCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleTaxonomyRepository>();

        await repository.AddAsync(new VehicleTaxonomyDocument()
        {
            Id = id,
            Name = name,
            EntityType = VehicleTaxonomyEntityType.Make
        });

        var context = new TestLambdaContext();
        var makesApi = new MakesApi();

        var result = await makesApi.DeleteMakeHandler(
            deleteMakeCommandHandler,
            id,
            context
            );

        var dbRecord = await repository.GetByIdAsync(VehicleTaxonomyEntityType.Make, id, null);
        var output = result.SerializeToText();

        Assert.Null(dbRecord);
        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:null,\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }
}
