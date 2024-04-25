namespace VehicleTaxonomy.Aws.Infrastructure.Db;

public interface IVehicleTaxonomyRepository
{
    Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(
        VehicleTaxonomyEntityType entityType,
        string? parentEntityId,
        CancellationToken cancellationToken = default
        );

    Task<VehicleTaxonomyDocument?> GetByIdAsync(
        VehicleTaxonomyEntityType entityType,
        string id,
        string? parentId,
        CancellationToken cancellationToken = default
        );

    Task AddAsync(
        VehicleTaxonomyDocument taxonomy,
        CancellationToken cancellationToken = default
        );

    Task DeleteByIdAsync(
        VehicleTaxonomyEntityType entityType,
        string id,
        string? parentId,
        CancellationToken cancellationToken = default
        );
}
