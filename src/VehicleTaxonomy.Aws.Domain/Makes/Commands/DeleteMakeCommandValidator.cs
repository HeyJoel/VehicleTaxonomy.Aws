using FluentValidation;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class DeleteMakeCommandValidator : AbstractValidator<DeleteMakeCommand>
{
    public DeleteMakeCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().MaximumLength(50).IsSlugId();
    }
}
