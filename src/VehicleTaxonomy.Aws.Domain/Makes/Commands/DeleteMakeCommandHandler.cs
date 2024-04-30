using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class DeleteMakeCommandHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public DeleteMakeCommandHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<CommandResponse> ExecuteAsync(DeleteMakeCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validator = new DeleteMakeCommandValidator();
        var result = validator.Validate(command);

        if (!result.IsValid)
        {
            return CommandResponse.Error(result);
        }

        var existing = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntity.Make,
            command.MakeId,
            null,
            null,
            cancellationToken
            );

        if (existing is null)
        {
            return CommandResponse.Error("Make could not be found.", nameof(command.MakeId));
        }

        await _vehicleTaxonomyRepository.DeleteByIdAsync(
            VehicleTaxonomyEntity.Make,
            command.MakeId,
            null,
            null,
            cancellationToken
            );

        return CommandResponse.Success();
    }
}
