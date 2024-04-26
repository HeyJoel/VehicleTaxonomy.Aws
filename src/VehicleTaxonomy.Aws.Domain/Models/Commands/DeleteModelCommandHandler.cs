using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Models;

public class DeleteModelCommandHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public DeleteModelCommandHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<CommandResponse> ExecuteAsync(DeleteModelCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validator = new DeleteModelCommandValidator();
        var result = validator.Validate(command);

        if (!result.IsValid)
        {
            return CommandResponse.Error(result);
        }

        var existing = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntityType.Model,
            command.ModelId,
            command.MakeId,
            cancellationToken
            );

        if (existing is null)
        {
            return CommandResponse.Error("Model could not be found.", nameof(command.ModelId));
        }

        await _vehicleTaxonomyRepository.DeleteByIdAsync(
            VehicleTaxonomyEntityType.Model,
            command.ModelId,
            command.MakeId,
            cancellationToken
            );

        return CommandResponse.Success();
    }
}
