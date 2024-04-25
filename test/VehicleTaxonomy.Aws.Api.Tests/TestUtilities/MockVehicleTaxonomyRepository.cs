using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Api.Tests;

public class MockVehicleTaxonomyRepository : IVehicleTaxonomyRepository
{
    private readonly List<VehicleTaxonomyDocument> _db = [];

    public Task AddAsync(VehicleTaxonomyDocument taxonomy, CancellationToken cancellationToken = default)
    {
        _db.Add(taxonomy);

        return Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(VehicleTaxonomyEntityType entityType, string id, string? parentId, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(entityType, id, parentId, cancellationToken);
        if (item != null)
        {
            _db.Remove(item);
        }
    }

    public Task<VehicleTaxonomyDocument?> GetByIdAsync(VehicleTaxonomyEntityType entityType, string id, string? parentId, CancellationToken cancellationToken = default)
    {
        var item = _db.SingleOrDefault(d =>
            d.EntityType == entityType
            && d.Id == id
            && d.ParentId == parentId
            );

        return Task.FromResult(item);
    }

    public Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(VehicleTaxonomyEntityType entityType, string? parentEntityId, CancellationToken cancellationToken = default)
    {
        var results = _db
            .OrderBy(d => d.Name)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<VehicleTaxonomyDocument>>(results);
    }
}
