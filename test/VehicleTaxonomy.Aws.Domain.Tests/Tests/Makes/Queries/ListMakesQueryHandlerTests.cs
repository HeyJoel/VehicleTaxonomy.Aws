using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Makes;
using VehicleTaxonomy.Aws.Infrastructure;

namespace VehicleTaxonomy.Aws.Domain.Tests.Makes.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class ListMakesQueryHandlerTests
{
    const string UNIQUE_PREFIX = "ListMakesQH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public ListMakesQueryHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanReturnUnfiltered()
    {
        const string name1 = UNIQUE_PREFIX + nameof(CanReturnUnfiltered);
        const string name2 = name1 + "2";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ListMakesQueryHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        await makeTestHelper.AddMakeAsync(name2);
        await makeTestHelper.AddMakeAsync(name1);

        var response = await handler.ExecuteAsync(new());

        var filteredResults = EnumerableHelper
            .Enumerate(response.Result)
            .Where(r => r.Name.StartsWith(name1))
            .ToArray();

        using (new AssertionScope())
        {
            response.IsValid.Should().BeTrue();
            response.Result.Should().NotBeNullOrEmpty();

            InlineSnapshot.Validate(filteredResults, """
                - MakeId: listmakesqh-canreturnunfiltered
                  Name: ListMakesQH_CanReturnUnfiltered
                - MakeId: listmakesqh-canreturnunfiltered2
                  Name: ListMakesQH_CanReturnUnfiltered2
                """);
        }
    }

    [Fact]
    public async Task CanFilterByName()
    {
        const string namePrefix = UNIQUE_PREFIX + nameof(CanFilterByName);
        const string name1 = namePrefix + "1";
        const string name2 = namePrefix + "Two";
        const string name21 = namePrefix + "twoOne";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ListMakesQueryHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        await makeTestHelper.AddMakeAsync(name1);
        await makeTestHelper.AddMakeAsync(name2);
        await makeTestHelper.AddMakeAsync(name21);

        var response = await handler.ExecuteAsync(new()
        {
            Name = name2
        });

        var filteredResults = EnumerableHelper
            .Enumerate(response.Result)
            .Where(r => r.Name.StartsWith(namePrefix))
            .ToArray();

        using (new AssertionScope())
        {
            response.IsValid.Should().BeTrue();
            response.Result.Should().NotBeNullOrEmpty();

            InlineSnapshot.Validate(filteredResults, """
                - MakeId: listmakesqh-canfilterbynametwo
                  Name: ListMakesQH_CanFilterByNameTwo
                - MakeId: listmakesqh-canfilterbynametwoone
                  Name: ListMakesQH_CanFilterByNametwoOne
                """);
        }
    }
}
