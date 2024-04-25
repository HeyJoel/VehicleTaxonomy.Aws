using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace VehicleTaxonomy.Aws.Infrastructure.Db;

/// <summary>
/// Helper for initializing DynamoDb tables. This is currently
/// intended mainly for standing up local test databases and only
/// considers scheme configuration. For real databases we expect to
/// using SAM or CDK.
/// </summary>
public class DynamoDbTableInitializer
{
    private readonly IAmazonDynamoDB _client;

    public DynamoDbTableInitializer(
        IAmazonDynamoDB client
        )
    {
        _client = client;
    }

    public async Task CreateIfNotExistsAsync(
        IDynamoDbTableDefinition definition,
        Action<CreateTableRequest>? configureRequest = null
        )
    {
        if (await ExistsAsync(definition))
        {
            await CreateAsync(definition, configureRequest);
        }
    }

    public async Task RebuildAsync(
        IDynamoDbTableDefinition definition,
        Action<CreateTableRequest>? configureRequest = null
        )
    {
        if (await ExistsAsync(definition))
        {
            await _client.DeleteTableAsync(definition.TableName);
        }

        await CreateAsync(definition, configureRequest);
    }

    private async Task<bool> ExistsAsync(IDynamoDbTableDefinition definition)
    {
        var tables = await _client.ListTablesAsync();
        return tables.TableNames.Contains(definition.TableName);
    }

    private async Task CreateAsync(
        IDynamoDbTableDefinition definition,
        Action<CreateTableRequest>? configureRequest = null
        )
    {
        // Provisioned-throughput settings are always required,
        // although the local test version of DynamoDB ignores them
        // so let's set a default for test environments
        var defaultProvisionedThroughput = new ProvisionedThroughput(1, 1);

        var request = new CreateTableRequest()
        {
            TableName = definition.TableName,
            AttributeDefinitions = definition.Attributes.ToList(),
            KeySchema = definition.KeySchema.ToList(),
            GlobalSecondaryIndexes = definition
                .GlobalSecondaryIndexes
                .Select(s =>
                {
                    s.ProvisionedThroughput ??= defaultProvisionedThroughput;
                    return s;
                })
                .ToList(),
            ProvisionedThroughput = defaultProvisionedThroughput
        };

        configureRequest?.Invoke(request);

        await _client.CreateTableAsync(request);
    }
}
