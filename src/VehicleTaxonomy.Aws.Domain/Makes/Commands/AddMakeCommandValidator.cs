using FluentValidation;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class AddMakeCommandValidator : AbstractValidator<AddMakeCommand>
{
    public AddMakeCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(50);
    }
}
