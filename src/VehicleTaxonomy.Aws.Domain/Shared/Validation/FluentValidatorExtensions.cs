using System.Text.RegularExpressions;
using FluentValidation;

namespace VehicleTaxonomy.Aws.Domain;

public static partial class FluentValidatorExtensions
{
    /// <summary>
    /// Validates that a string is a "slug" style identifier i.e.
    /// only contains lowercase letters (a-z), numbers or "-" dashes.
    /// </summary>
    public static IRuleBuilderOptions<T, string> IsSlugId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(s => string.IsNullOrWhiteSpace(s) || SlugIdRegex().IsMatch(s))
            .WithMessage("Ids should contain only lowercase letters (a-z), numbers or dashes");
    }

    [GeneratedRegex("^[a-z0-9-]+$")]
    private static partial Regex SlugIdRegex();
}
