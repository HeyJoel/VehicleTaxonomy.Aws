using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace VehicleTaxonomy.Aws.Infrastructure.Db;

/// <summary>
/// Definition used for creating the tample in automated tests.
/// Changes here should be reflexted in the SAM template in the root of
/// the repository which is used to define the schema for the deployed
/// application.
/// </summary>
public class VehicleTaxonomyTableDefinition : IDynamoDbTableDefinition
{
    public string TableName { get; } = "VehicleTaxonomy";

    public IReadOnlyCollection<AttributeDefinition> Attributes { get; } =
    [
        new AttributeDefinition(nameof(VehicleTaxonomy.PK), ScalarAttributeType.S),
        new AttributeDefinition(nameof(VehicleTaxonomy.SK), ScalarAttributeType.S),
        new AttributeDefinition(nameof(VehicleTaxonomy.MakeId), ScalarAttributeType.S)
    ];

    public IReadOnlyCollection<KeySchemaElement> KeySchema { get; } =
    [
        new KeySchemaElement(nameof(VehicleTaxonomy.PK), KeyType.HASH),
        new KeySchemaElement(nameof(VehicleTaxonomy.SK), KeyType.RANGE)
    ];

    public IReadOnlyCollection<GlobalSecondaryIndex> GlobalSecondaryIndexes { get; } =
    [
        new GlobalSecondaryIndex()
        {
            IndexName = "Makes",
            KeySchema = [new KeySchemaElement(nameof(VehicleTaxonomy.MakeId), KeyType.HASH)],
            Projection = new()
            {
                ProjectionType = ProjectionType.INCLUDE,
                NonKeyAttributes = [nameof(VehicleTaxonomy.Name), nameof(VehicleTaxonomy.CreateDate)]
            }
        }
    ];

    public static readonly VehicleTaxonomyTableDefinition Instance = new();

    public const int MakeNameMaxLength = 50;
    public const int ModelNameMaxLength = 50;
    public const int VariantNameMaxLength = 100;
}
