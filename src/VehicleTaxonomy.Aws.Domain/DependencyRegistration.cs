using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.DataImport;
using VehicleTaxonomy.Aws.Domain.Makes;
using VehicleTaxonomy.Aws.Domain.Models;
using VehicleTaxonomy.Aws.Domain.Variants;

namespace VehicleTaxonomy.Aws.Domain;

public static class DependencyRegistration
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddTransient<ListMakesQueryHandler>()
            .AddTransient<IsMakeUniqueQueryHandler>()
            .AddTransient<AddMakeCommandHandler>()
            .AddTransient<DeleteMakeCommandHandler>()
            .AddTransient<ListModelsQueryHandler>()
            .AddTransient<IsModelUniqueQueryHandler>()
            .AddTransient<AddModelCommandHandler>()
            .AddTransient<DeleteModelCommandHandler>()
            .AddTransient<ListVariantsQueryHandler>()
            .AddTransient<IsVariantUniqueQueryHandler>()
            .AddTransient<AddVariantCommandHandler>()
            .AddTransient<DeleteVariantCommandHandler>()
            .AddTransient<ImportTaxonomyFromCsvCommandHandler>()
            .AddTransient<TaxonomyFromCsvImportJob>()
            ;

        return services;
    }
}
