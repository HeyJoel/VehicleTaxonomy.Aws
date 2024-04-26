using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Models;

public class DeleteModelCommandValidator : AbstractValidator<DeleteModelCommand>
{
    public DeleteModelCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.MakeNameMaxLength);
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.ModelNameMaxLength);
    }
}
