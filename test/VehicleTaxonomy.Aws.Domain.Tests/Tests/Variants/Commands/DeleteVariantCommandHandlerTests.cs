using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Variants;

namespace VehicleTaxonomy.Aws.Domain.Tests.Variants.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class DeleteVariantCommandHandlerTests
{
    const string UNIQUE_PREFIX = "DelVariantCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public DeleteVariantCommandHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanDelete()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(CanDelete);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteVariantCommandHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (makeId, modelId) = await variantTestHelper.AddModelWithMakeAsync(uniqueData);
        var id = await variantTestHelper.AddVariantAsync(makeId, modelId, uniqueData);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            VariantId = id
        });

        var dbRecord = await variantTestHelper.GetRawRecordAsync(makeId, modelId, id);

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            dbRecord.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenModelIdInvalid_ReturnsError(string? modelId)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = modelId!,
            VariantId = "na"
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteVariantCommand.ModelId));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenIdInvalid_ReturnsError(string? id)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = "na",
            VariantId = id!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteVariantCommand.VariantId));
        }
    }

    [Fact]
    public async Task WhenNotExists_ReturnsError()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenNotExists_ReturnsError);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(uniqueData),
            ModelId = EntityIdFormatter.Format(uniqueData),
            VariantId = EntityIdFormatter.Format(uniqueData)
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);

            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteVariantCommand.VariantId));
            error.Message.Should().Match("*not*found*");
        }
    }
}
