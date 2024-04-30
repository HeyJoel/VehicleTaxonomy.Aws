using FluentValidation;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.DataImport;

public class TaxonomyCsvRowValidator : AbstractValidator<TaxonomyCsvRow>
{
    public TaxonomyCsvRowValidator()
    {
        // Note that the slug will never be longer than the name length so we don't need to
        // validate it here, preventing duplicate errors
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId();
        RuleFor(c => c.MakeName).NotEmpty().MaximumLength(VehicleTaxonomyTableDefinition.MakeNameMaxLength).WithMessage(StandardErrorMessages.StringMaxLength);

        RuleFor(c => c.ModelId).NotEmpty().IsSlugId();
        RuleFor(c => c.ModelName).NotEmpty().MaximumLength(VehicleTaxonomyTableDefinition.ModelNameMaxLength).WithMessage(StandardErrorMessages.StringMaxLength);

        RuleFor(c => c.VariantId).NotEmpty().IsSlugId();
        RuleFor(c => c.VariantName).NotEmpty().MaximumLength(VehicleTaxonomyTableDefinition.VariantNameMaxLength).WithMessage(StandardErrorMessages.StringMaxLength);

        RuleFor(c => c.EngineSizeInCC).LessThan(50000);
    }
}
