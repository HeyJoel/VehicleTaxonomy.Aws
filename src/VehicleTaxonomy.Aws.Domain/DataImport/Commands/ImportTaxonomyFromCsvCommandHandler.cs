using VehicleTaxonomy.Aws.Domain.Shared.FileSources;
using VehicleTaxonomy.Aws.Infrastructure.DataImport;

namespace VehicleTaxonomy.Aws.Domain.DataImport;

public class ImportTaxonomyFromCsvCommandHandler
{
    private readonly ILogger<ImportTaxonomyFromCsvCommandHandler> _logger;
    private readonly TaxonomyFromCsvImportJob _taxonomyFromCsvImportJob;
    private readonly CsvDataImportJobRunner _csvDataImportJobRunner;

    public ImportTaxonomyFromCsvCommandHandler(
        ILogger<ImportTaxonomyFromCsvCommandHandler> logger,
        TaxonomyFromCsvImportJob taxonomyFromCsvImportJob,
        CsvDataImportJobRunner csvDataImportJobRunner
        )
    {
        _logger = logger;
        _taxonomyFromCsvImportJob = taxonomyFromCsvImportJob;
        _csvDataImportJobRunner = csvDataImportJobRunner;
    }

    public async Task<CommandResponse<DataImportJobResult>> ExecuteAsync(ImportTaxonomyFromCsvCommand command, CancellationToken cancellationToken = default)
    {
        var validator = new ImportTaxonomyFromCsvCommandValidator();
        var validationResult = validator.Validate(command);
        if (!validationResult.IsValid)
        {
            return CommandResponse<DataImportJobResult>.Error(validationResult);
        }

        using var stream = await command.File.TryOpenReadStreamAsync(_logger);
        if (stream == null)
        {
            return CommandResponse<DataImportJobResult>.Error("Error opening file.", nameof(command.File));
        }

        var importTask = command.ImportMode switch
        {
            DataImportMode.Validate => _csvDataImportJobRunner.ValidateAsync(stream, _taxonomyFromCsvImportJob, cancellationToken),
            DataImportMode.Run => _csvDataImportJobRunner.RunAsync(stream, _taxonomyFromCsvImportJob, cancellationToken),
            _ => throw new NotImplementedException($"Unknown {nameof(DataImportMode)} '{command.ImportMode}'")
        };

        var result = await importTask;

        return CommandResponse<DataImportJobResult>.Success(result);
    }
}
