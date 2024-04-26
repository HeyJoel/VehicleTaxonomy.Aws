using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.DynamoDb;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Tests;

/// <summary>
/// Fixture for tests that need to access dynamo db. The dynamo
/// db fixture should be re-used across multiple tests, using
/// unique keys to scope data to each test.
/// </summary>
public sealed class DbDependentFixture : IAsyncLifetime
{
    private readonly DynamoDbContainer _container = new DynamoDbBuilder().Build();

    /// <summary>
    /// We use a constant seed date for reproducable results.
    /// </summary>
    public DateTimeOffset SeedDate => new(2024, 04, 24, 13, 56, 33, TimeSpan.Zero);

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ServiceProvider = CreateServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var dynamoDbTableInitializer = scope.ServiceProvider.GetRequiredService<DynamoDbTableInitializer>();
        await dynamoDbTableInitializer.RebuildAsync(VehicleTaxonomyTableDefinition.Instance);
    }

    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private ServiceProvider CreateServiceProvider()
    {
        var configuration = GetConfiguration();
        var services = new ServiceCollection();
        services
            .AddDomainTests(configuration)
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddScoped(s => new FakeTimeProvider(SeedDate))
            .AddScoped<TimeProvider>(s => s.GetRequiredService<FakeTimeProvider>());

        return services.BuildServiceProvider();
    }

    private IConfiguration GetConfiguration()
    {
        var dynamoConnectionString = _container.GetConnectionString();

        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddInMemoryCollection([
                new($"{DynamoDbOptions.SectionName}:{nameof(DynamoDbOptions.UseLocalDb)}", "true"),
                new($"{DynamoDbOptions.SectionName}:{nameof(DynamoDbOptions.LocalDbServiceUrl)}", dynamoConnectionString),
            ])
            .Build();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
