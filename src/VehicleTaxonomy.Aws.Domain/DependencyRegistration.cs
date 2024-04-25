using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Makes;
using VehicleTaxonomy.Aws.Infrastructure;

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
            ;

        return services;
    }
}
