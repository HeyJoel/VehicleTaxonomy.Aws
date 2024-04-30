using VehicleTaxonomy.Aws.Domain.Variants;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Api.Tests;

/// <summary>
/// Basic endpoint tests. Logic is tested in the domain test project.
/// </summary>
[Collection(nameof(ServiceDependentFixture))]
public class VariantApiTests
{
    private const string UNIQUE_DATA = nameof(VariantApiTests);

    private readonly ServiceDependentFixture _serviceDependentFixture;

    public VariantApiTests(
        ServiceDependentFixture serviceDependentFixture
        )
    {
        _serviceDependentFixture = serviceDependentFixture;
    }

    [Fact]
    public async Task ListVariants_CanQuery()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var listVariantsQueryHandler = scope.ServiceProvider.GetRequiredService<ListVariantsQueryHandler>();

        var context = new TestLambdaContext();
        var variantsApi = new VariantsApi();
        var result = await variantsApi.ListVariantsHandler(
            listVariantsQueryHandler,
            "volkswagen",
            "v-coupe",
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:[{\u0022VariantId\u0022:\u0022vc-buzzster-1-3-turbo\u0022,\u0022Name\u0022:\u0022VC Buzzster 1.3 Turbo\u0022,\u0022FuelCategory\u0022:null,\u0022EngineSizeInCC\u0022:null},{\u0022VariantId\u0022:\u0022vc-se-2-0\u0022,\u0022Name\u0022:\u0022VC SE 2.0\u0022,\u0022FuelCategory\u0022:null,\u0022EngineSizeInCC\u0022:null}],\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task IsVariantUnique_CanQuery()
    {
        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var isVariantUniqueQueryHandler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var context = new TestLambdaContext();
        var variantsApi = new VariantsApi();
        var result = await variantsApi.IsVariantUniqueHandler(
            isVariantUniqueQueryHandler,
            "volkswagen",
            "v-coupe",
            nameof(IsVariantUnique_CanQuery),
            context
            );

        var output = result.SerializeToText();

        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:true,\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task AddVariant_CanAdd()
    {
        var name = UNIQUE_DATA + nameof(AddVariant_CanAdd);
        var makeId = "audi";
        var modelId = "a-coupe";
        var variantId = EntityIdFormatter.Format(name);

        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var addVariantCommandHandler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleTaxonomyRepository>();

        var context = new TestLambdaContext();
        var variantsApi = new VariantsApi();
        var command = new AddVariantCommand()
        {
            Name = name
        };

        var result = await variantsApi.AddVariantHandler(
            addVariantCommandHandler,
            makeId,
            modelId,
            command,
            context
            );

        var dbRecord = await repository.GetByIdAsync(VehicleTaxonomyEntity.Variant, variantId, makeId, modelId);
        var output = result.SerializeToText();

        Assert.NotNull(dbRecord);
        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:{\u0022Id\u0022:\u0022variantapitestsaddvariant-canadd\u0022},\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }

    [Fact]
    public async Task AddVariant_CanDelete()
    {
        var name = UNIQUE_DATA + nameof(AddVariant_CanDelete);
        var variantId = EntityIdFormatter.Format(name);
        var makeId = "audi";
        var modelId = "a-coupe";

        using var scope = _serviceDependentFixture.ServiceProvider.CreateScope();
        var deleteVariantCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteVariantCommandHandler>();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleTaxonomyRepository>();

        await repository.AddAsync(new VehicleTaxonomyDocument()
        {
            Id = variantId,
            Name = name,
            ParentMakeId = makeId,
            ParentModelId = modelId,
            EntityType = VehicleTaxonomyEntity.Variant
        });

        var context = new TestLambdaContext();
        var variantsApi = new VariantsApi();

        var result = await variantsApi.DeleteVariantHandler(
            deleteVariantCommandHandler,
            makeId,
            modelId,
            variantId,
            context
            );

        var dbRecord = await repository.GetByIdAsync(VehicleTaxonomyEntity.Variant, variantId, makeId, modelId);
        var output = result.SerializeToText();

        Assert.Null(dbRecord);
        InlineSnapshot.Validate(output, """
            {"statusCode":200,"multiValueHeaders":{"content-type":["application/json"]},"body":"{\u0022IsValid\u0022:true,\u0022Result\u0022:null,\u0022ValidationErrors\u0022:[]}","isBase64Encoded":false}
            """);
    }
}
