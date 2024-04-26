using VehicleTaxonomy.Aws.Domain.Shared.Validation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Models;

public class IsModelUniqueQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public IsModelUniqueQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<bool>> ExecuteAsync(IsModelUniqueQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var modelId = EntityIdFormatter.Format(query.Name);
        var validationResult = await ValidateAsync(modelId, query, cancellationToken);

        if (validationResult is not null)
        {
            return validationResult;
        }

        var dbResult = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntityType.Model,
            modelId,
            query.MakeId,
            cancellationToken: cancellationToken
            );

        var result = dbResult == null;

        return QueryResponse<bool>.Success(result);
    }

    private async Task<QueryResponse<bool>?> ValidateAsync(
        string modelId,
        IsModelUniqueQuery query,
        CancellationToken cancellationToken
        )
    {
        // Validate basic
        var validator = new IsModelUniqueQueryValidator();
        var validationResult = validator.Validate(query);
        if (!validationResult.IsValid)
        {
            return QueryResponse<bool>.Error(validationResult);
        }

        // Validate modelId
        if (string.IsNullOrEmpty(modelId))
        {
            return QueryResponse<bool>.Error(
                StandardErrorMessages.NameCouldNotBeFormattedAsAnId,
                nameof(query.Name)
                );
        }

        // Validate parent make exists
        var make = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntityType.Make,
            query.MakeId,
            null,
            cancellationToken
            );
        if (make == null)
        {
            return QueryResponse<bool>.Error(
                "Make does not exist.",
                nameof(query.MakeId)
                );
        }

        return null;
    }
}
