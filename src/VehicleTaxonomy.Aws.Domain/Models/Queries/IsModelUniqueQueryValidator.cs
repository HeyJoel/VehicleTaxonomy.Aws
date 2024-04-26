using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Models;

public class IsModelUniqueQueryValidator : AbstractValidator<IsModelUniqueQuery>
{
    public IsModelUniqueQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.MakeNameMaxLength);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyTableDefinition.ModelNameMaxLength);
    }
}
