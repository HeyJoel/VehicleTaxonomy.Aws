using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class ListMakesQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public ListMakesQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<IReadOnlyCollection<Make>>> ExecuteAsync(ListMakesQuery? query, CancellationToken cancellationToken = default)
    {
        IEnumerable<VehicleTaxonomyDocument> dbResults = await _vehicleTaxonomyRepository.ListAsync(
            VehicleTaxonomyEntity.Make,
            null,
            null,
            cancellationToken: cancellationToken
            );

        if (!string.IsNullOrWhiteSpace(query?.Name))
        {
            // I'm not sure whether filtering in a DynamoDb scan operation would
            // be better, worse or much the same. I believe it works in a similar iterative way
            // but the filtering is done on the server. I'd need to do a bit more research to be sure.
            // In any case, DynamoDb isn't designed for this sort of filtering
            dbResults = dbResults.Where(d => d.Name.StartsWith(query.Name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        var results = dbResults
            .Select(r => new Make()
            {
                MakeId = r.Id,
                Name = r.Name
            })
            .ToArray();

        return QueryResponse<IReadOnlyCollection<Make>>.Success(results);
    }
}
