using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using VehicleTaxonomy.Aws.Domain.Models;
using VehicleTaxonomy.Aws.Domain.Tests.Makes;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Tests.Models;

public class ModelTestHelper
{
    private readonly IAmazonDynamoDB _amazonDynamoDB;
    private readonly AddModelCommandHandler _addModelCommandHandler;
    private readonly MakeTestHelper _makeTestHelper;

    public ModelTestHelper(
        IAmazonDynamoDB amazonDynamoDB,
        AddModelCommandHandler addModelCommandHandler,
        MakeTestHelper makeTestHelper
        )
    {
        _amazonDynamoDB = amazonDynamoDB;
        _addModelCommandHandler = addModelCommandHandler;
        _makeTestHelper = makeTestHelper;
    }

    /// <summary>
    /// Returns the raw dynamo db record data for a vehicle model.
    /// </summary>
    public async Task<Dictionary<string, AttributeValue>?> GetRawRecordAsync(string makeId, string id)
    {
        var key = new Dictionary<string, AttributeValue>()
        {
            {  "PK", new($"make#{makeId}#models") },
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
        return await _makeTestHelper.AddMakeAsync(name);
    }

    public async Task<string> AddModelAsync(string makeId, string name)
    {
        var result = await _addModelCommandHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        result.ThrowIfInvalid();

        return result.Result!.Id;
    }
}
