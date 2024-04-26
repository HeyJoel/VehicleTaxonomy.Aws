using Amazon.DynamoDBv2.DataModel;

namespace VehicleTaxonomy.Aws.Infrastructure.Db;

[DynamoDBTable(nameof(VehicleTaxonomy))]
internal class VehicleTaxonomy
{
    /// <summary>
    /// The Partition Key makeup depends on the entity e.g. for
    /// make "make#{MakeId}", for model "make#{MakeId}#models"
    /// or for variants "model#{ModelId}#variants". This allows
    /// querying by PK to list makes and models while retaining
    /// good cardinality. Make listing are done via a GSI on the
    /// <see cref="MakeId"/> field.
    /// </summary>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    /// <summary>
    /// The sort key is always the entity id e.g. for make "volkswagen"
    /// for model "volkswagen-polo" for variant "volkswagen-polo-polo-sport-1-3-petrol".
    /// This allows the PK and SK to form a unique combination
    /// </summary>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    /// <summary>
    /// Publically displayable name or title of the record e.g. "BMW" or
    /// "3 Series".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A copy of the make id (also stored in SK), but only present for makes
    /// allowing for a sparse GSI that can be used to query all makes.
    /// </summary>
    [DynamoDBGlobalSecondaryIndexHashKey("Makes")]
    public string? MakeId { get; set; }

    /// <summary>
    /// The date the record was created in UTC.
    /// </summary>
    public DateTime CreateDate { get; set; }
}
