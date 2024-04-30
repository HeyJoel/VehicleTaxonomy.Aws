using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using VehicleTaxonomy.Aws.Domain.Tests.Models;
using VehicleTaxonomy.Aws.Domain.Variants;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Tests.Variants;

public class VariantTestHelper
{
    private readonly IAmazonDynamoDB _amazonDynamoDB;
    private readonly AddVariantCommandHandler _addVariantCommandHandler;
    private readonly ModelTestHelper _modelTestHelper;

    public VariantTestHelper(
        IAmazonDynamoDB amazonDynamoDB,
        AddVariantCommandHandler addVariantCommandHandler,
        ModelTestHelper modelTestHelper
        )
    {
        _amazonDynamoDB = amazonDynamoDB;
        _addVariantCommandHandler = addVariantCommandHandler;
        _modelTestHelper = modelTestHelper;
    }

    /// <summary>
    /// Returns the raw dynamo db record data for a vehicle variant.
    /// </summary>
    public async Task<Dictionary<string, AttributeValue>?> GetRawRecordAsync(string makeId, string modelId, string id)
    {
        var key = new Dictionary<string, AttributeValue>()
        {
            {  "PK", new($"make#{makeId}#model#{modelId}#variants") },
            {  "SK", new(id) }
        };

        var dbRecord = await _amazonDynamoDB.GetItemAsync(
            VehicleTaxonomyTableDefinition.Instance.TableName,
            key,
            true
            );

        return dbRecord.Item.Count == 0 ? null : dbRecord.Item;
    }

    public async Task<(string, string)> AddModelWithMakeAsync(string name)
    {
        var makeId = await _modelTestHelper.AddMakeAsync(name + "mk");
        var modelId = await _modelTestHelper.AddModelAsync(makeId, name);

        return (makeId, modelId);
    }

    public async Task<string> AddVariantAsync(string makeId, string modelId, string name, Action<AddVariantCommand>? configureCommand = null)
    {
        var command = new AddVariantCommand()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        };

        configureCommand?.Invoke(command);
        var result = await _addVariantCommandHandler.ExecuteAsync(command);
        result.ThrowIfInvalid();

        return result.Result!.Id;
    }
}
