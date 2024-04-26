using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Models;

namespace VehicleTaxonomy.Aws.Domain.Tests.Models.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsModelUniqueQueryHandlerTests
{
    const string UNIQUE_PREFIX = "IsModelUniqueQH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public IsModelUniqueQueryHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenUnique_ReturnsTrue()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenUnique_ReturnsTrue);
        var name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();
        var make1Id = await modelTestHelper.AddMakeAsync(uniqueData);
        var make2Id = await modelTestHelper.AddMakeAsync(uniqueData + "ignored");
        await modelTestHelper.AddModelAsync(make1Id, name + "ignored");
        await modelTestHelper.AddModelAsync(make2Id, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = make1Id,
            Name = name
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            result.Result.Should().BeTrue();
        }
    }

    [Fact]
    public async Task WhenMakeNotExists_ReturnsFalse()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenMakeNotExists_ReturnsFalse);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(uniqueData),
            Name = uniqueData
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsModelUniqueQuery.MakeId));
            error.Message.Should().MatchEquivalentOf("*make*exist*");
            result.Result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenMakeIdInvalid_ReturnsError(string? id)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id!,
            Name = "na"
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsModelUniqueQuery.MakeId));
            result.Result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("Lorem ipsum dolor amet, consectetur adipiscing elit")]
    public async Task WhenNameInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            Name = name!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsModelUniqueQuery.Name));
            result.Result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsFalse()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenNameNotUnique_ReturnsFalse);
        const string name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var makeId = await modelTestHelper.AddMakeAsync(uniqueData);
        await modelTestHelper.AddModelAsync(makeId, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            result.Result.Should().BeFalse();
        }
    }
}
