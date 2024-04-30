namespace VehicleTaxonomy.Aws.Infrastructure.Db;

/// <summary>
/// Light abstraction of data access for all taxonomy types i.e. makes
/// models and variants. This abstracts some of the peculiarities of
/// working with the structure of the taxonomy DynamoDb table.
/// </summary>
public interface IVehicleTaxonomyRepository
{
    /// <summary>
    /// Returns all taxonomies of the specified <see cref="VehicleTaxonomyEntity"/>.
    /// Models and variant types must be filtered by their parent ids; returning unfiltered
    /// models or variants is not supported.
    /// </summary>
    /// <param name="entityType">
    /// The type of entity to filter by.
    /// </param>
    /// <param name="parentMakeId">
    /// For <see cref="VehicleTaxonomyEntity.Model"/> and <see cref="VehicleTaxonomyEntity.Variant"/>
    /// you must include the id of the parent make to filter on. You cannot return
    /// all models or variants.
    /// </param>
    /// <param name="parentModelId">
    /// For <see cref="VehicleTaxonomyEntity.Variant"/> you must include the id of
    /// the parent make and model to filter on. You cannot return all variants.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to pass down to any async data access
    /// requests.
    /// </param>
    Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(
        VehicleTaxonomyEntity entityType,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Returns a single <paramref name="entityType"/> using the specified
    /// <paramref name="entityId"/> and parent id combination. If no entity
    /// is found then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="entityType">
    /// The type of entity to return.
    /// </param>
    /// <param name="entityId">
    /// The unique id of the <paramref name="entityType"/> to return.
    /// </param>
    /// <param name="parentMakeId">
    /// For <see cref="VehicleTaxonomyEntity.Model"/> and <see cref="VehicleTaxonomyEntity.Variant"/>
    /// you must include the id of the parent make to filter on.
    /// </param>
    /// <param name="parentModelId">
    /// For <see cref="VehicleTaxonomyEntity.Variant"/> you must include the id of
    /// the parent make and model to filter on.
    /// <param name="cancellationToken">
    /// Optional cancellation token to pass down to any async data access
    /// requests.
    /// </param>
    Task<VehicleTaxonomyDocument?> GetByIdAsync(
        VehicleTaxonomyEntity entityType,
        string entityId,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Adds a new document to the data store. No check is made to
    /// see if the document already exists.
    /// </summary>
    /// <param name="taxonomy">
    /// Document to add.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to pass down to any async data access
    /// requests.
    /// </param>
    Task AddAsync(
        VehicleTaxonomyDocument taxonomy,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Adds a batch of documents to the data store. The repository will
    /// handle batching of writes to the underlaying data store if necessary.
    /// </summary>
    /// <param name="taxonomies">
    /// Documents to write.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to pass down to any async data access
    /// requests.
    /// </param>
    Task AddBatchAsync(
        IEnumerable<VehicleTaxonomyDocument> taxonomies,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Deletes a taxonomy record using the specified <paramref name="entityId"/>
    /// and parent id combination. It does not check that the entity exists before
    /// a deletion is made.
    /// </summary>
    /// <param name="entityType">
    /// The type of entity to return.
    /// </param>
    /// <param name="entityId">
    /// The unique id of the <paramref name="entityType"/> to delete.
    /// </param>
    /// <param name="parentMakeId">
    /// For <see cref="VehicleTaxonomyEntity.Model"/> and <see cref="VehicleTaxonomyEntity.Variant"/>
    /// you must include the id of the parent make to filter on.
    /// </param>
    /// <param name="parentModelId">
    /// For <see cref="VehicleTaxonomyEntity.Variant"/> you must include the id of
    /// the parent make and model to filter on.
    /// <param name="cancellationToken">
    /// Optional cancellation token to pass down to any async data access
    /// requests.
    /// </param>
    Task DeleteByIdAsync(
        VehicleTaxonomyEntity entityType,
        string entityId,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        );
}
