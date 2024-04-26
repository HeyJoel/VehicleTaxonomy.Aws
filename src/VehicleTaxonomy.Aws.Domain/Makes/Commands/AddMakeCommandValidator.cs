using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class AddMakeCommandValidator : AbstractValidator<AddMakeCommand>
{
    public AddMakeCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyTableDefinition.MakeNameMaxLength);
    }
}
