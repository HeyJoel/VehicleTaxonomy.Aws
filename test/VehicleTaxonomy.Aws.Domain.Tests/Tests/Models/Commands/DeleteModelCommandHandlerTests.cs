using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Models;

namespace VehicleTaxonomy.Aws.Domain.Tests.Models.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class DeleteModelCommandHandlerTests
{
    const string UNIQUE_PREFIX = "DelModelCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public DeleteModelCommandHandlerTests(
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
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var makeId = await modelTestHelper.AddMakeAsync(uniqueData);
        var id = await modelTestHelper.AddModelAsync(makeId, uniqueData);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = id
        });

        var dbRecord = await modelTestHelper.GetRawRecordAsync(makeId, id);

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
    public async Task WhenMakeIdInvalid_ReturnsError(string? makeId)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId!,
            ModelId = "na"
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteModelCommand.MakeId));
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
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = id!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteModelCommand.ModelId));
        }
    }

    [Fact]
    public async Task WhenNotExists_ReturnsError()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenNotExists_ReturnsError);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(uniqueData),
            ModelId = EntityIdFormatter.Format(uniqueData)
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);

            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteModelCommand.ModelId));
            error.Message.Should().Match("*not*found*");
        }
    }
}
