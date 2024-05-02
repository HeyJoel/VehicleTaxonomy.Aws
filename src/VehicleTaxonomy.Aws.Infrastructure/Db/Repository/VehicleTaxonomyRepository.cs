using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace VehicleTaxonomy.Aws.Infrastructure.Db;

/// <summary>
/// DynamoDb implementation of <see cref="IVehicleTaxonomyRepository"/>.
/// </summary>
internal sealed class VehicleTaxonomyRepository : IVehicleTaxonomyRepository, IDisposable
{
    private readonly DynamoDBContext _dynamoDBContext;

    public VehicleTaxonomyRepository(IAmazonDynamoDB client)
    {
        _dynamoDBContext = new(client, new()
        {
            // Table strucutre specified in code so no need to fetch
            DisableFetchingTableMetadata = true,
            RetrieveDateTimeInUtc = true
        });
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(
        VehicleTaxonomyEntity entityType,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        AsyncSearch<VehicleTaxonomy> asyncSearch;

        if (entityType == VehicleTaxonomyEntity.Make)
        {
            // The Makes GSI is a sparse index that only contains makes
            // so we can use scan to get all items
            asyncSearch = _dynamoDBContext.FromScanAsync<VehicleTaxonomy>(new ScanOperationConfig()
            {
                IndexName = "Makes",
                Select = SelectValues.AllProjectedAttributes
            });
        }
        else
        {
            var pk = BuildPartitionKey(entityType, "NA", parentMakeId, parentModelId);
            asyncSearch = _dynamoDBContext.QueryAsync<VehicleTaxonomy>(pk);
        }

        var result = await asyncSearch.GetNextSetAsync(cancellationToken);
        var additionalResults = await asyncSearch.GetRemainingAsync(cancellationToken);

        return result
            .Concat(additionalResults)
            .Select(r => MapToDocument(entityType, r))
            .OrderBy(t => t.Name)
            .ToArray();
    }

    /// <inheritdoc/>
    public async Task<VehicleTaxonomyDocument?> GetByIdAsync(
        VehicleTaxonomyEntity entityType,
        string id,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        var pk = BuildPartitionKey(entityType, id, parentMakeId, parentModelId);
        var result = await _dynamoDBContext.LoadAsync<VehicleTaxonomy>(pk, id, cancellationToken);

        if (result == null)
        {
            return null;
        }

        var mapped = MapToDocument(entityType, result);

        return mapped;
    }

    /// <inheritdoc/>
    public async Task AddAsync(
        VehicleTaxonomyDocument taxonomy,
        CancellationToken cancellationToken = default
        )
    {
        var dbTaxonomy = MapToDbTaxonomy(taxonomy);

        await _dynamoDBContext.SaveAsync(dbTaxonomy, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddBatchAsync(
        IEnumerable<VehicleTaxonomyDocument> taxonomies,
        CancellationToken cancellationToken = default
        )
    {
        var batchWrite = _dynamoDBContext.CreateBatchWrite<VehicleTaxonomy>();
        var dbTaxonomies = taxonomies.Select(MapToDbTaxonomy);
        batchWrite.AddPutItems(dbTaxonomies);
        await _dynamoDBContext.ExecuteBatchWriteAsync([batchWrite], cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteByIdAsync(
        VehicleTaxonomyEntity entityType,
        string id,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        var pk = BuildPartitionKey(entityType, id, parentMakeId, parentModelId);
        await _dynamoDBContext.DeleteAsync<VehicleTaxonomy>(pk, id, cancellationToken);
    }

    private static string BuildPartitionKey(
        VehicleTaxonomyEntity entityType,
        string id,
        string? parentMakeId,
        string? parentModelId
        )
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        if (entityType != VehicleTaxonomyEntity.Make)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentMakeId);
        }
        if (entityType == VehicleTaxonomyEntity.Variant)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentModelId);
        }

        var key = entityType switch
        {
            VehicleTaxonomyEntity.Make => $"make#{id}",
            VehicleTaxonomyEntity.Model => $"make#{parentMakeId}#models",
            VehicleTaxonomyEntity.Variant => $"make#{parentMakeId}#model#{parentModelId}#variants",
            _ => throw new NotImplementedException($"Unknown {nameof(VehicleTaxonomyEntity)} value {entityType}")
        };

        return key;
    }

    private static VehicleTaxonomyDocument MapToDocument(
        VehicleTaxonomyEntity entityType,
        VehicleTaxonomy dbVehicleTaxonomy
        )
    {
        var id = dbVehicleTaxonomy.SK;

        var result = new VehicleTaxonomyDocument()
        {
            EntityType = entityType,
            Id = id,
            Name = dbVehicleTaxonomy.Name,
            CreateDate = dbVehicleTaxonomy.CreateDate
        };

        switch (entityType)
        {
            case VehicleTaxonomyEntity.Make:
                // If data is from Makes GSI, the id is not included in the index
                // because we can use the MakeId index key value instead
                if (string.IsNullOrEmpty(id))
                {
                    if (string.IsNullOrEmpty(dbVehicleTaxonomy.MakeId))
                    {
                        throw new InvalidOperationException($"{nameof(dbVehicleTaxonomy.MakeId)} should not be empty for a make.");
                    }
                    result.Id = dbVehicleTaxonomy.MakeId;
                }
                break;
            case VehicleTaxonomyEntity.Model:
                // e.g. make#volkswagen#models
                var modelPkParts = GetPartitionKeyParts(3);
                result.ParentModelId = modelPkParts[1];
                break;
            case VehicleTaxonomyEntity.Variant:
                // e.g. make#volkswagen#model#bmw-3-series#variants
                var variantPkParts = GetPartitionKeyParts(5);
                result.ParentMakeId = variantPkParts[1];
                result.ParentModelId = variantPkParts[3];
                result.VariantData = new()
                {
                    EngineSizeInCC = dbVehicleTaxonomy.EngineSizeInCC,
                    FuelCategory = dbVehicleTaxonomy.FuelCategory
                };
                break;
            default:
                throw new NotImplementedException($"Unknown taxonomy entity {entityType}.");
        }

        return result;

        string[] GetPartitionKeyParts(int length)
        {
            var pkParts = dbVehicleTaxonomy.PK.Split('#');

            if (pkParts.Length != length)
            {
                throw new InvalidOperationException($"Invalid number of partition key elements found in {entityType} key {dbVehicleTaxonomy.PK}");
            }

            return pkParts;
        }
    }

    private static VehicleTaxonomy MapToDbTaxonomy(VehicleTaxonomyDocument taxonomy)
    {
        var dbTaxonomy = new VehicleTaxonomy()
        {
            PK = BuildPartitionKey(taxonomy.EntityType, taxonomy.Id, taxonomy.ParentMakeId, taxonomy.ParentModelId),
            SK = taxonomy.Id,
            Name = taxonomy.Name,
            MakeId = taxonomy.EntityType == VehicleTaxonomyEntity.Make ? taxonomy.Id : null,
            CreateDate = taxonomy.CreateDate
        };

        if (taxonomy.EntityType == VehicleTaxonomyEntity.Variant && taxonomy.VariantData != null)
        {
            dbTaxonomy.EngineSizeInCC = taxonomy.VariantData.EngineSizeInCC;
            dbTaxonomy.FuelCategory = taxonomy.VariantData.FuelCategory;
        }

        return dbTaxonomy;
    }

    public void Dispose()
    {
        _dynamoDBContext?.Dispose();
    }
}
