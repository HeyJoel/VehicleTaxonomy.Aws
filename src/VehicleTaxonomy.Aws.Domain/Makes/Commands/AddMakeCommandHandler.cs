using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class AddMakeCommandHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;
    private readonly IsMakeUniqueQueryHandler _isMakeUniqueQueryHandler;
    private readonly TimeProvider _timeProvider;

    public AddMakeCommandHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository,
        IsMakeUniqueQueryHandler isMakeUniqueQueryHandler,
        TimeProvider timeProvider
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
        _isMakeUniqueQueryHandler = isMakeUniqueQueryHandler;
        _timeProvider = timeProvider;
    }

    public async Task<CommandResponse<AddEntityResult>> ExecuteAsync(AddMakeCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var id = EntityIdFormatter.Format(command.Name);
        var validationResult = await ValidateAsync(command, id, cancellationToken);
        if (validationResult != null)
        {
            return validationResult;
        }

        var now = _timeProvider.GetUtcNow().DateTime;
        await _vehicleTaxonomyRepository.AddAsync(new()
        {
            CreateDate = now,
            EntityType = VehicleTaxonomyEntity.Make,
            Id = id,
            Name = command.Name.Trim()
        }, cancellationToken);

        return CommandResponse<AddEntityResult>.Success(new()
        {
            Id = id
        });
    }

    private async Task<CommandResponse<AddEntityResult>?> ValidateAsync(
        AddMakeCommand command,
        string id,
        CancellationToken cancellationToken
        )
    {
        // Basic Validation
        var validator = new AddMakeCommandValidator();
        var result = validator.Validate(command);

        if (!result.IsValid)
        {
            return CommandResponse<AddEntityResult>.Error(result);
        }

        // Id Validation
        if (string.IsNullOrEmpty(id))
        {
            return CommandResponse<AddEntityResult>.Error(
                StandardErrorMessages.NameCouldNotBeFormattedAsAnId,
                nameof(command.Name)
                );
        }

        // Uniqueness Validation
        var isUniqueResult = await _isMakeUniqueQueryHandler.ExecuteAsync(new()
        {
            Name = command.Name
        }, cancellationToken);

        isUniqueResult.ThrowIfInvalid();

        if (!isUniqueResult.Result)
        {
            return CommandResponse<AddEntityResult>.Error(
                StandardErrorMessages.NameIsNotUnique("make"),
                nameof(command.Name));
        }

        return null;
    }
}
