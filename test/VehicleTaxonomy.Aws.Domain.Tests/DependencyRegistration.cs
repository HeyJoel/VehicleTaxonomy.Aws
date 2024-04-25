using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.Tests.Makes;

namespace VehicleTaxonomy.Aws.Domain;

public static class DependencyRegistration
{
    public static IServiceCollection AddDomainTests(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDomain(configuration)
            .AddTransient<MakeTestHelper>()
            ;

        return services;
    }
}
