using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Makes;

namespace VehicleTaxonomy.Aws.Domain.Tests.Makes.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class DeleteMakeCommandHandlerTests
{
    const string UNIQUE_PREFIX = "DelMakeCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public DeleteMakeCommandHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanDelete()
    {
        const string name = UNIQUE_PREFIX + nameof(CanDelete);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        var id = await makeTestHelper.AddMakeAsync(name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id
        });

        var dbRecord = await makeTestHelper.GetRawRecordAsync(id);

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
    public async Task WhenIdInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = name!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteMakeCommand.MakeId));
        }
    }

    [Fact]
    public async Task WhenNotExists_ReturnsError()
    {
        const string name = UNIQUE_PREFIX + nameof(WhenNotExists_ReturnsError);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(name)
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);

            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(DeleteMakeCommand.MakeId));
            error.Message.Should().Match("*not*found*");
        }
    }
}
