using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Variants;

public class DeleteVariantCommandHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public DeleteVariantCommandHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<CommandResponse> ExecuteAsync(DeleteVariantCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validator = new DeleteVariantCommandValidator();
        var result = validator.Validate(command);

        if (!result.IsValid)
        {
            return CommandResponse.Error(result);
        }

        var existing = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntityType.Variant,
            command.VariantId,
            command.ModelId,
            cancellationToken
            );

        if (existing is null)
        {
            return CommandResponse.Error("Variant could not be found.", nameof(command.VariantId));
        }

        await _vehicleTaxonomyRepository.DeleteByIdAsync(
            VehicleTaxonomyEntityType.Variant,
            command.VariantId,
            command.ModelId,
            cancellationToken
            );

        return CommandResponse.Success();
    }
}
