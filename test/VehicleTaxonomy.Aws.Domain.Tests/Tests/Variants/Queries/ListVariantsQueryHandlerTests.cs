using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Variants;
using VehicleTaxonomy.Aws.Infrastructure;

namespace VehicleTaxonomy.Aws.Domain.Tests.Variants.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class ListVariantsQueryHandlerTests
{
    const string UNIQUE_PREFIX = "ListVariantsQH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public ListVariantsQueryHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanListByMake()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(CanListByMake);
        const string name1 = uniqueData + "1";
        const string name2 = name1 + "2";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ListVariantsQueryHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (make1Id, model1Id) = await variantTestHelper.AddModelWithMakeAsync(uniqueData);
        var (make2Id, model2Id) = await variantTestHelper.AddModelWithMakeAsync(uniqueData + "ignored");
        await variantTestHelper.AddVariantAsync(make2Id, model2Id, uniqueData + "ignore");
        await variantTestHelper.AddVariantAsync(make1Id, model1Id, name1);
        await variantTestHelper.AddVariantAsync(make1Id, model1Id, name2, c =>
        {
            c.EngineSizeInCC = 1234;
            c.FuelCategory = FuelCategory.ElectricHybridDiesel;
        });

        var response = await handler.ExecuteAsync(new()
        {
            MakeId = make1Id,
            ModelId = model1Id
        });

        var filteredResults = EnumerableHelper
            .Enumerate(response.Result)
            .Where(r => r.Name.StartsWith(name1))
            .ToArray();

        using (new AssertionScope())
        {
            response.IsValid.Should().BeTrue();
            response.Result.Should().NotBeNullOrEmpty();

            InlineSnapshot.Validate(filteredResults, """
                - VariantId: listvariantsqh-canlistbymake1
                  Name: ListVariantsQH_CanListByMake1
                - VariantId: listvariantsqh-canlistbymake12
                  Name: ListVariantsQH_CanListByMake12
                """);
        }
    }
}
