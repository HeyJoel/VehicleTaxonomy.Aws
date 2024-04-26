using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Models;

public class ListModelsQueryValidator : AbstractValidator<ListModelsQuery>
{
    public ListModelsQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.MakeNameMaxLength);
    }
}
