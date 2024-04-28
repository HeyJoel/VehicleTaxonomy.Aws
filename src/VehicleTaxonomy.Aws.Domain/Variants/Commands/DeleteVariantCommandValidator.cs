using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.Variants;

public class DeleteVariantCommandValidator : AbstractValidator<DeleteVariantCommand>
{
    public DeleteVariantCommandValidator()
    {
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.ModelNameMaxLength);
        RuleFor(c => c.VariantId).NotEmpty().IsSlugId(VehicleTaxonomyTableDefinition.VariantNameMaxLength);
    }
}
