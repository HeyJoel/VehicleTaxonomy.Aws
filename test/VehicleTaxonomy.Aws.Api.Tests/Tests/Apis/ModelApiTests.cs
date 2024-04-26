using VehicleTaxonomy.Aws.Domain.Models;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Api.Tests;

/// <summary>
/// Basic endpoint tests. Logic is tested in the domain test project.
/// </summary>
[Collection(nameof(ServiceDependentFixture))]
public class ModelApiTests
{
    private const string UNIQUE_DATA = nameof(ModelApiTests);

    private readonly ServiceDependentFixture _serviceDependentFixture;

    public ModelApiTests(
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
    public async Task ListModels_CanQuery()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var listModelsQueryHandler = scope.ServiceProvider.GetRequiredService<ListModelsQueryHandler>();

        var context = new TestLambdaContext();
        var modelsApi = new ModelsApi();
        var result = await modelsApi.ListModelsHandler(
            listModelsQueryHandler,
            "volkswagen",
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:[{\u0022ModelId\u0022:\u0022v-coupe\u0022,\u0022Name\u0022:\u0022V Coupe\u0022},{\u0022ModelId\u0022:\u0022v-saloon\u0022,\u0022Name\u0022:\u0022V Saloon\u0022}],\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task IsModelUnique_CanQuery()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var isModelUniqueQueryHandler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var context = new TestLambdaContext();
        var modelsApi = new ModelsApi();
        var result = await modelsApi.IsModelUniqueHandler(
            isModelUniqueQueryHandler,
            "volkswagen",
            nameof(IsModelUnique_CanQuery),
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:true,\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task AddModel_CanAdd()
    {
        var name = UNIQUE_DATA + nameof(AddModel_CanAdd);
        var makeId = "audi";
        var modelId = EntityIdFormatter.Format(name);

        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var addModelCommandHandler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleTaxonomyRepository>();

        var context = new TestLambdaContext();
        var modelsApi = new ModelsApi();
        var command = new AddModelCommand()
        {
            Name = name
        };

        var result = await modelsApi.AddModelHandler(
            addModelCommandHandler,
            makeId,
            command,
            context
            );

        var dbRecord = await repository.GetByIdAsync(VehicleTaxonomyEntityType.Model, modelId, makeId);
        var output = result.SerializeToText();

        Assert.NotNull(dbRecord);
        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:{\u0022Id\u0022:\u0022modelapitestsaddmodel-canadd\u0022},\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task AddModel_CanDelete()
    {
        var name = UNIQUE_DATA + nameof(AddModel_CanDelete);
        var modelId = EntityIdFormatter.Format(name);
        var makeId = "audi";

        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var deleteModelCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleTaxonomyRepository>();

        await repository.AddAsync(new VehicleTaxonomyDocument()
        {
            Id = modelId,
            Name = name,
            ParentId = "audi",
            EntityType = VehicleTaxonomyEntityType.Model
        });

        var context = new TestLambdaContext();
        var modelsApi = new ModelsApi();

        var result = await modelsApi.DeleteModelHandler(
            deleteModelCommandHandler,
            makeId,
            modelId,
            context
            );

        var dbRecord = await repository.GetByIdAsync(VehicleTaxonomyEntityType.Model, modelId, null);
        var output = result.SerializeToText();

        Assert.Null(dbRecord);
        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:null,\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }
}
