using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using VehicleTaxonomy.Aws.Domain.Makes;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Tests.Makes;

public class MakeTestHelper
{
    private readonly IAmazonDynamoDB _amazonDynamoDB;
    private readonly AddMakeCommandHandler _addMakeCommandHandler;

    public MakeTestHelper(
        IAmazonDynamoDB amazonDynamoDB,
        AddMakeCommandHandler addMakeCommandHandler
        )
    {
        _amazonDynamoDB = amazonDynamoDB;
        _addMakeCommandHandler = addMakeCommandHandler;
    }

    /// <summary>
    /// Returns the raw dynamo db record data for a Make.
    /// </summary>
    public async Task<Dictionary<string, AttributeValue>?> GetRawRecordAsync(string id)
    {
        var key = new Dictionary<string, AttributeValue>()
        {
            {  "PK", new($"make#{id}") },
            {  "SK", new(id) }
        };

        var dbRecord = await _amazonDynamoDB.GetItemAsync(
            VehicleTaxonomyTableDefinition.Instance.TableName,
            key,
            true
            );

        return dbRecord.Item.Count == 0 ? null : dbRecord.Item;
    }
    public async Task<string> AddMakeAsync(string name)
    {
        var result = await _addMakeCommandHandler.ExecuteAsync(new()
        {
            Name = name
        });

        result.ThrowIfInvalid();

        return result.Result!.Id;
    }
}
