using Amazon.DynamoDBv2.Model;

namespace VehicleTaxonomy.Aws.Infrastructure.Db;

/// <summary>
/// Defines a basic table schema for use with
/// <see cref="DynamoDbTableInitializer"/>.
/// </summary>
public interface IDynamoDbTableDefinition
{
    string TableName { get; }

    /// <summary>
    /// Note that you only need to define the attributes used in
    /// <see cref="KeySchema"/> or <see cref="GlobalSecondaryIndexes"/>, non-key
    /// attributes don't need to be initialized. If you define non-key attributes
    /// DynamoDb the AWS client will throw an exception.
    /// </summary>
    IReadOnlyCollection<AttributeDefinition> Attributes { get; }

    /// <summary>
    /// Elements used to define the table key i.e. Partition Key (AKA PK or HASH) and
    /// optionally the Sort Key (AKA SK or RANGE).
    /// </summary>
    IReadOnlyCollection<KeySchemaElement> KeySchema { get; }

    /// <summary>
    /// Optionally you can define global secondary indexes (GSI) here.
    /// </summary>
    IReadOnlyCollection<GlobalSecondaryIndex> GlobalSecondaryIndexes { get; }
}
