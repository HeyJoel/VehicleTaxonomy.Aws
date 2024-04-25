using FluentValidation;

namespace VehicleTaxonomy.Aws.Domain.Makes;

public class IsMakeUniqueQueryValidator : AbstractValidator<IsMakeUniqueQuery>
{
    public IsMakeUniqueQueryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(50);
    }
}
