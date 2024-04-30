using FluentValidation;

namespace VehicleTaxonomy.Aws.Domain.DataImport;

public class ImportTaxonomyFromCsvCommandValidator : AbstractValidator<ImportTaxonomyFromCsvCommand>
{
    public ImportTaxonomyFromCsvCommandValidator()
    {
        RuleFor(c => c.File).NotNull();
    }
}
