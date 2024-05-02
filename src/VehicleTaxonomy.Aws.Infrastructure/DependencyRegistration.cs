using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Infrastructure.DataImport;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Infrastructure;

public static class DependencyRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dynamoDbOptions = new DynamoDbOptions();
        configuration.GetSection("DynamoDb").Bind(dynamoDbOptions);

        if (dynamoDbOptions.UseLocalDb)
        {
            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                // Provide dummy credentials, so the AWS SDK doesn't go looking
                // for them elsewhere and throw an error
                // https://github.com/aws/aws-sdk-net/issues/1878
                var credentials = new BasicAWSCredentials("NA", "NA");
                var clientConfig = new AmazonDynamoDBConfig()
                {
                    ServiceURL = dynamoDbOptions.LocalDbServiceUrl
                };
                return new AmazonDynamoDBClient(credentials, clientConfig);
            });
        }
        else
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();
        }

        services
            .AddTransient<IVehicleTaxonomyRepository, VehicleTaxonomyRepository>()
            .AddTransient<DynamoDbTableInitializer>()
            .AddTransient<CsvDataImportJobRunner>();

        return services;
    }
}
