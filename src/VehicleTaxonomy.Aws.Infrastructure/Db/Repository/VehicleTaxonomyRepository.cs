using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace VehicleTaxonomy.Aws.Infrastructure.Db;

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

    public async Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(
        VehicleTaxonomyEntityType entityType,
        string? parentEntityId,
        CancellationToken cancellationToken = default
        )
    {
        AsyncSearch<VehicleTaxonomy> asyncSearch;

        if (entityType == VehicleTaxonomyEntityType.Make)
        {
            // The Makes GSI is a sparse index that only contains makes
            // sowe can use scan to get all items
            asyncSearch = _dynamoDBContext.FromScanAsync<VehicleTaxonomy>(new ScanOperationConfig()
            {
                IndexName = "Makes",
                Select = SelectValues.AllProjectedAttributes
            });
        }
        else
        {
            var pk = BuildPartitionKey(entityType, "NA", parentEntityId);
            asyncSearch = _dynamoDBContext.QueryAsync<VehicleTaxonomy>(pk);
        }

        var result = await asyncSearch.GetNextSetAsync(cancellationToken);
        var additionalResults = await asyncSearch.GetRemainingAsync(cancellationToken);

        return result
            .Concat(additionalResults)
            .Select(r => Map(entityType, r))
            .OrderBy(t => t.Name)
            .ToArray();
    }

    public async Task<VehicleTaxonomyDocument?> GetByIdAsync(
        VehicleTaxonomyEntityType entityType,
        string id,
        string? parentId,
        CancellationToken cancellationToken = default
        )
    {
        var pk = BuildPartitionKey(entityType, id, parentId);
        var result = await _dynamoDBContext.LoadAsync<VehicleTaxonomy>(pk, id, cancellationToken);

        if (result == null)
        {
            return null;
        }

        var mapped = Map(entityType, result);

        return mapped;
    }

    public async Task AddAsync(
        VehicleTaxonomyDocument taxonomy,
        CancellationToken cancellationToken = default
        )
    {
        var dbTaxonomy = new VehicleTaxonomy()
        {
            PK = BuildPartitionKey(taxonomy.EntityType, taxonomy.Id, taxonomy.ParentId),
            SK = taxonomy.Id,
            Name = taxonomy.Name,
            MakeId = taxonomy.EntityType == VehicleTaxonomyEntityType.Make ? taxonomy.Id : null,
            CreateDate = taxonomy.CreateDate
        };

        await _dynamoDBContext.SaveAsync(dbTaxonomy, cancellationToken);
    }

    public async Task DeleteByIdAsync(
        VehicleTaxonomyEntityType entityType,
        string id,
        string? parentId,
        CancellationToken cancellationToken = default
        )
    {
        var pk = BuildPartitionKey(entityType, id, parentId);
        await _dynamoDBContext.DeleteAsync<VehicleTaxonomy>(pk, id, cancellationToken);
    }

    private static string BuildPartitionKey(
        VehicleTaxonomyEntityType entityType,
        string id,
        string? parentId
        )
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        if (entityType != VehicleTaxonomyEntityType.Make)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentId);
        }

        var key = entityType switch
        {
            VehicleTaxonomyEntityType.Make => $"make#{id}",
            VehicleTaxonomyEntityType.Model => $"make#{parentId}#models",
            VehicleTaxonomyEntityType.Variant => $"model#{parentId}#variants",
            _ => throw new NotImplementedException($"Unknown {nameof(VehicleTaxonomyEntityType)} value {entityType}")
        };

        return key;
    }

    private static VehicleTaxonomyDocument Map(
        VehicleTaxonomyEntityType entityType,
        VehicleTaxonomy dbVehicleTaxonomy
        )
    {
        var id = dbVehicleTaxonomy.SK;
        string? parentId = null;

        if (entityType == VehicleTaxonomyEntityType.Make)
        {
            // If data is from Makes GSI, the id is not included in the index
            // this is because we can use the MakeId index key value instead
            if (string.IsNullOrEmpty(id))
            {
                if (string.IsNullOrEmpty(dbVehicleTaxonomy.MakeId))
                {
                    throw new InvalidOperationException($"{nameof(dbVehicleTaxonomy.MakeId)} should not be empty for a make.");
                }
                id = dbVehicleTaxonomy.MakeId;
            }
        }
        else
        {
            // Parse the PK to get the parent id
            // e.g. make#volkswagen#models
            // e.g. model#bmw-3-series#variants
            var pkParts = dbVehicleTaxonomy.PK.Split('#');

            if (pkParts.Length is not 3)
            {
                throw new InvalidOperationException($"Invalid number of partition key elements found in key {dbVehicleTaxonomy.PK}");
            }

            parentId = pkParts[1];
        }

        return new VehicleTaxonomyDocument()
        {
            EntityType = entityType,
            Id = id,
            ParentId = parentId,
            Name = dbVehicleTaxonomy.Name,
            CreateDate = dbVehicleTaxonomy.CreateDate
        };
    }

    public void Dispose()
    {
        _dynamoDBContext?.Dispose();
    }
}
