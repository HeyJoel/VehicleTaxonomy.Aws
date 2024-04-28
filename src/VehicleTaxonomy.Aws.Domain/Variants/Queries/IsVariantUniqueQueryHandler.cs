using VehicleTaxonomy.Aws.Domain.Shared.Validation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Variants;

public class IsVariantUniqueQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public IsVariantUniqueQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<bool>> ExecuteAsync(IsVariantUniqueQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var variantId = EntityIdFormatter.Format(query.Name);
        var validationResult = await ValidateAsync(variantId, query, cancellationToken);

        if (validationResult is not null)
        {
            return validationResult;
        }

        var dbResult = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntityType.Variant,
            variantId,
            query.ModelId,
            cancellationToken: cancellationToken
            );

        var result = dbResult == null;

        return QueryResponse<bool>.Success(result);
    }

    private async Task<QueryResponse<bool>?> ValidateAsync(
        string variantId,
        IsVariantUniqueQuery query,
        CancellationToken cancellationToken
        )
    {
        // Validate basic
        var validator = new IsVariantUniqueQueryValidator();
        var validationResult = validator.Validate(query);
        if (!validationResult.IsValid)
        {
            return QueryResponse<bool>.Error(validationResult);
        }

        // Validate variantId
        if (string.IsNullOrEmpty(variantId))
        {
            return QueryResponse<bool>.Error(
                StandardErrorMessages.NameCouldNotBeFormattedAsAnId,
                nameof(query.Name)
                );
        }

        // Validate parent model exists
        var model = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntityType.Model,
            query.ModelId,
            query.MakeId,
            cancellationToken
            );
        if (model == null)
        {
            return QueryResponse<bool>.Error(
                "Model does not exist.",
                nameof(query.ModelId)
                );
        }

        return null;
    }
}
