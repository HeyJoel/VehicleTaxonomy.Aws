using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Makes;

namespace VehicleTaxonomy.Aws.Domain.Tests.Makes.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsMakeUniqueQueryHandlerTests
{
    const string UNIQUE_PREFIX = "IsMakeUniqueQH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public IsMakeUniqueQueryHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenUnique_ReturnsTrue()
    {
        const string name = UNIQUE_PREFIX + nameof(WhenUnique_ReturnsTrue);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            Name = name
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            result.Result.Should().BeTrue();
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
        var handler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            Name = name!
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(IsMakeUniqueQuery.Name));
            result.Result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsFalse()
    {
        const string name = UNIQUE_PREFIX + nameof(WhenNameNotUnique_ReturnsFalse);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();

        await makeTestHelper.AddMakeAsync(name);

        var result = await handler.ExecuteAsync(new()
        {
            Name = name
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            result.Result.Should().BeFalse();
        }
    }
}
