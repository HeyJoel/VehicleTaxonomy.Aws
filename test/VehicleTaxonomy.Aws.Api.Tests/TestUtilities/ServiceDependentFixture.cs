using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Api.Tests;

/// <summary>
/// Reusable fixture that sets up the standard DI container.
/// </summary>
public class ServiceDependentFixture
{
    public IServiceProvider ServiceProvider { get; private set; } = CreateServiceProvider();

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        var startup = new Startup();
        startup.ConfigureServices(services);

        services.AddScoped<IVehicleTaxonomyRepository>(s => GetSeededRepository());

        return services.BuildServiceProvider();
    }

    private static MockVehicleTaxonomyRepository GetSeededRepository()
    {
        var repository = new MockVehicleTaxonomyRepository();
        string[] makes = ["Volkswagen", "Audi", "MG"];
        string[] models = ["Coupe", "Saloon"];
        string[] variants = ["SE 2.0", "Buzzster 1.3 Turbo"];
        var now = new DateTime(2024, 1, 1, 16, 25, 12, DateTimeKind.Utc);

        foreach (var make in makes)
        {
            var makeId = EntityIdFormatter.Format(make);
            var makePrefix = make.First();
            repository.AddAsync(new()
            {
                CreateDate = now,
                EntityType = VehicleTaxonomyEntityType.Make,
                Id = makeId,
                Name = make
            });

            foreach (var model in models)
            {
                var modelName = $"{makePrefix} {model}";
                var modelId = EntityIdFormatter.Format(modelName);
                var modelPrefix = model.First();
                repository.AddAsync(new()
                {
                    CreateDate = now,
                    EntityType = VehicleTaxonomyEntityType.Model,
                    Id = modelId,
                    Name = modelName,
                    ParentId = makeId
                });

                foreach (var variant in variants)
                {
                    var variantName = $"{makePrefix}{modelPrefix} {variant}";
                    repository.AddAsync(new()
                    {
                        CreateDate = now,
                        EntityType = VehicleTaxonomyEntityType.Variant,
                        Id = EntityIdFormatter.Format(variantName),
                        Name = variantName,
                        ParentId = modelId
                    });
                }
            }
        }

        return repository;
    }
}
