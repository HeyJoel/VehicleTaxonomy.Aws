namespace VehicleTaxonomy.Aws.Infrastructure.Db;

/// <summary>
/// Abstraction over the <see cref="VehicleTaxonomy"/> DynamoDb
/// record to hide away some of the akward aspects of the overloaded
/// schema and make it more pleasant to work with.
/// </summary>
public class VehicleTaxonomyDocument
{
    /// <summary>
    /// Used to determine the entity type in the overload
    /// dynamo db table.
    /// </summary>
    public VehicleTaxonomyEntityType EntityType { get; set; }

    /// <summary>
    /// Unique id of the entity i.e. MakeId, ModelId or VariantId
    /// e.g. "volkswagen", "3-series" or "polo-se-petrol-1-3".
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Unique <see cref="Id"/> of the parent entity i.e for a Model
    /// this will be the MakeId (e.g. "bmw") or for a variant this will
    /// be the ModelId (e.g. "3-series").
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Publically displayable name or title of the record e.g. "BMW" or
    /// "3 Series".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The date the record was created, in UTC.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Additional fields relating only to "variant" entities.
    /// </summary>
    public VariantData? VariantData { get; set; }
}
