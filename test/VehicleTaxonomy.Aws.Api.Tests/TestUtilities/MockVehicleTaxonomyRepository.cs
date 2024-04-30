using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Api.Tests;

public class MockVehicleTaxonomyRepository : IVehicleTaxonomyRepository
{
    private readonly List<VehicleTaxonomyDocument> _db = [];

    public Task AddAsync(
        VehicleTaxonomyDocument taxonomy,
        CancellationToken cancellationToken = default
        )
    {
        _db.Add(taxonomy);

        return Task.CompletedTask;
    }

    public async Task AddBatchAsync(
        IEnumerable<VehicleTaxonomyDocument> taxonomies,
        CancellationToken cancellationToken = default
        )
    {
        foreach (var taxonomy in taxonomies)
        {
            await AddAsync(taxonomy, cancellationToken);
        }
    }

    public async Task DeleteByIdAsync(
        VehicleTaxonomyEntity entityType,
        string entityId,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(entityType, entityId, parentMakeId, parentModelId, cancellationToken);
        if (item != null)
        {
            _db.Remove(item);
        }
    }

    public Task<VehicleTaxonomyDocument?> GetByIdAsync(
        VehicleTaxonomyEntity entityType,
        string entityId,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        var item = _db.SingleOrDefault(d =>
            d.EntityType == entityType
            && d.Id == entityId
            && d.ParentMakeId == parentMakeId
            && d.ParentModelId == parentModelId
            );

        return Task.FromResult(item);
    }

    public Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(
        VehicleTaxonomyEntity entityType,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        var results = _db
            .Where(d =>
                d.EntityType == entityType
                && d.ParentMakeId == parentMakeId
                && d.ParentModelId == parentModelId
                )
            .OrderBy(d => d.Name)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<VehicleTaxonomyDocument>>(results);
    }
}
