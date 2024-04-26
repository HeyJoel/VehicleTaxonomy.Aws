using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class IsMakeUniqueQueryValidator : AbstractValidator<IsMakeUniqueQuery>
{
    public IsMakeUniqueQueryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyTableDefinition.MakeNameMaxLength);
    }
}
