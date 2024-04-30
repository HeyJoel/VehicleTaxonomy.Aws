using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Variants;

public class ListVariantsQueryValidator : AbstractValidator<ListVariantsQuery>
{
    public ListVariantsQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.MakeNameMaxLength);
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.ModelNameMaxLength);
    }
}
