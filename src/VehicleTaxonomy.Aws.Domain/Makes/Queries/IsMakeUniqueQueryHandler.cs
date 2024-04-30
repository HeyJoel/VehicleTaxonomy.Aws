using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class IsMakeUniqueQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public IsMakeUniqueQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<bool>> ExecuteAsync(IsMakeUniqueQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var validator = new IsMakeUniqueQueryValidator();
        var validationResult = validator.Validate(query);

        if (!validationResult.IsValid)
        {
            return QueryResponse<bool>.Error(validationResult);
        }

        var makeId = EntityIdFormatter.Format(query.Name);

        if (string.IsNullOrEmpty(makeId))
        {
            return QueryResponse<bool>.Error(
                StandardErrorMessages.NameCouldNotBeFormattedAsAnId,
                nameof(query.Name)
                );
        }

        var dbResult = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntity.Make,
            makeId,
            null,
            null,
            cancellationToken: cancellationToken
            );

        var result = dbResult == null;

        return QueryResponse<bool>.Success(result);
    }
}
