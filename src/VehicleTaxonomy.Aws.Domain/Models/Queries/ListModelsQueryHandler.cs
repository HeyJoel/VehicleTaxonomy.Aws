using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Models;

public class ListModelsQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public ListModelsQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<IReadOnlyCollection<Model>>> ExecuteAsync(ListModelsQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<VehicleTaxonomyDocument> dbResults = await _vehicleTaxonomyRepository.ListAsync(
            VehicleTaxonomyEntityType.Model,
            query.MakeId,
            cancellationToken: cancellationToken
            );

        var results = dbResults
            .Select(r => new Model()
            {
                ModelId = r.Id,
                Name = r.Name
            })
            .ToArray();

        return QueryResponse<IReadOnlyCollection<Model>>.Success(results);
    }
}
