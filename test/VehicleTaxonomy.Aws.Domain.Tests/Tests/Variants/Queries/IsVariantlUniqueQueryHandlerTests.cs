using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Variants;

namespace VehicleTaxonomy.Aws.Domain.Tests.Variants.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsVariantUniqueQueryHandlerTests
{
    const string UNIQUE_PREFIX = "IsVariantUniqueQH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public IsVariantUniqueQueryHandlerTests(
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
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();
        var (make1Id, model1Id) = await variantTestHelper.AddModelWithMakeAsync(uniqueData);
        var (make2Id, model2Id) = await variantTestHelper.AddModelWithMakeAsync(uniqueData + "ignored");
        await variantTestHelper.AddVariantAsync(make1Id, model1Id, name + "ignored");
        await variantTestHelper.AddVariantAsync(make2Id, model2Id, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = make1Id,
            ModelId = model1Id,
            Name = name
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            result.Result.Should().BeTrue();
        }
    }

    [Fact]
    public async Task WhenModelNotExists_ReturnsFalse()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenModelNotExists_ReturnsFalse);
        var id = EntityIdFormatter.Format(uniqueData);
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id,
            ModelId = id,
            Name = uniqueData
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsVariantUniqueQuery.ModelId));
            error.Message.Should().MatchEquivalentOf("*model*exist*");
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
    public async Task WhenModelIdInvalid_ReturnsError(string? id)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = id!,
            Name = "na"
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsVariantUniqueQuery.ModelId));
            result.Result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("Lorem ipsum dolor amet, consectetur adipiscing elit Lorem ipsum dolor amet, consectetur adipiscing el")]
    public async Task WhenNameInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = "na",
            Name = name!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsVariantUniqueQuery.Name));
            result.Result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsFalse()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenNameNotUnique_ReturnsFalse);
        const string name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (makeId, modelId) = await variantTestHelper.AddModelWithMakeAsync(uniqueData);
        await variantTestHelper.AddVariantAsync(makeId, modelId, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            result.Result.Should().BeFalse();
        }
    }
}
