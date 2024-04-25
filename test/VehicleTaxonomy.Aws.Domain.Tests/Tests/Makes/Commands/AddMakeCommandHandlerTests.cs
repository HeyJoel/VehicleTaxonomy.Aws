using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Makes;

namespace VehicleTaxonomy.Aws.Domain.Tests.Makes.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddMakeCommandHandlerTests
{
    const string UNIQUE_PREFIX = "AddMakeCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public AddMakeCommandHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValid_CanAdd()
    {
        const string name = UNIQUE_PREFIX + nameof(WhenValid_CanAdd);
        const string id = "addmakech-whenvalid-canadd";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();

        var result = await handler.ExecuteAsync(new()
        {
            Name = name
        });

        var dbRecord = await makeTestHelper.GetRawRecordAsync(id);

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            if (!result.IsValid)
            {
                return;
            }

            result.Result.Id.Should().Be(id);
            dbRecord.Should().NotBeNull();

            InlineSnapshot
                .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
                .Validate(dbRecord, """
                    SK:
                      S: addmakech-whenvalid-canadd
                    PK:
                      S: make#addmakech-whenvalid-canadd
                    CreateDate:
                      S: 2024-04-24T12:56:33.000Z
                    Name:
                      S: AddMakeCH_WhenValid_CanAdd
                    MakeId:
                      S: addmakech-whenvalid-canadd
                    """);
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
        var handler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            Name = name!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddMakeCommand.Name));
            result.Result.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsError()
    {
        const string name = UNIQUE_PREFIX + nameof(WhenNameNotUnique_ReturnsError);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();

        var result1 = await handler.ExecuteAsync(new()
        {
            Name = name
        });

        var result2 = await handler.ExecuteAsync(new()
        {
            Name = name
        });

        using (new AssertionScope())
        {
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ValidationErrors.Should().HaveCount(1);

            var error = result2.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddMakeCommand.Name));
            error.Message.Should().Match("*already exists*");
        }
    }
}
