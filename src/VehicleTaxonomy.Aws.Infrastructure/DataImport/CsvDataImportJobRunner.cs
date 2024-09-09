using nietras.SeparatedValues;

namespace VehicleTaxonomy.Aws.Infrastructure.DataImport;

/// <summary>
/// Used to run an <see cref="ICsvDataImportJob{TMappedRecord}"/>, taking care
/// of the coordination of the ETL process.
/// </summary>
public class CsvDataImportJobRunner
{
    private readonly ILogger<CsvDataImportJobRunner> _logger;

    public CsvDataImportJobRunner(
        ILogger<CsvDataImportJobRunner> logger
        )
    {
        _logger = logger;
    }

    /// <summary>
    /// Run a <paramref name="job"/> performing only the mapping and validation
    /// stages i.e. skipping the <see cref="ICsvDataImportJob{TMappedRecord}.SaveAsync(IEnumerable{TMappedRecord}, CancellationToken)"/>
    /// step.
    /// </summary>
    /// <typeparam name="TMappedRecord">
    /// The type of object output from the mapping (transform) stage of the
    /// import process.
    /// </typeparam>
    /// <param name="fileStream">
    /// The stream containing the csv file data. The stream is not closed or
    /// disposed of by the job runner.
    /// </param>
    /// <param name="job">The job to run.</param>
    /// <param name="cancellationToken">
    /// Cancellation token to pass on to downstream async services.
    /// </param>
    public async Task<DataImportJobResult> ValidateAsync<TMappedRecord>(
        Stream fileStream,
        ICsvDataImportJob<TMappedRecord> job,
        CancellationToken cancellationToken = default
        )
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(job);

        var results = await RunAsync(fileStream, job, true, cancellationToken);

        return results;
    }

    /// <summary>
    /// Run a <paramref name="job"/> using data from the specified
    /// <paramref name="fileStream"/>.
    /// </summary>
    /// <typeparam name="TMappedRecord">
    /// The type of object output from the mapping (transform) stage of the
    /// import process.
    /// </typeparam>
    /// <param name="fileStream">
    /// The stream containing the csv file data. The stream is not closed or
    /// disposed of by the job runner.
    /// </param>
    /// <param name="job">The job to run.</param>
    /// <param name="cancellationToken">
    /// Cancellation token to pass on to downstream async services.
    /// </param>
    public async Task<DataImportJobResult> RunAsync<TMappedRecord>(
        Stream fileStream,
        ICsvDataImportJob<TMappedRecord> job,
        CancellationToken cancellationToken = default
        )
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(job);

        var results = await RunAsync(fileStream, job, false, cancellationToken);

        return results;
    }

    private async Task<DataImportJobResult> RunAsync<TMappedRecord>(
        Stream fileStream,
        ICsvDataImportJob<TMappedRecord> job,
        bool validateOnly,
        CancellationToken cancellationToken = default
        )
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(job);

        var modeDescription = validateOnly ? "Validate" : "Run";
        var resultBuilder = new DataImportResultBuilder(_logger);
        resultBuilder.SetStatus(DataImportJobStatus.Processing);
        _logger.LogDebug("Import job started. Mode: {Mode}", modeDescription);

        using var reader = Sep.Reader().From(fileStream);

        var csvRows = reader.Enumerate((SepReader.Row row, out TMappedRecord? mappedRow) =>
        {
            _logger.LogTrace("Mapping row {RowIndex}", row.RowIndex);
            mappedRow = job.Map(resultBuilder, row);

            return mappedRow != null;
        }).Cast<TMappedRecord>();

        IEnumerable<TMappedRecord[]> batches;

        if (job.BatchSize > 0)
        {
            batches = csvRows.Chunk(job.BatchSize.Value);
        }
        else
        {
            batches = [csvRows.ToArray()];
        }

        var batchCount = 0;
        foreach (var batch in batches)
        {
            batchCount++;
            if (batch.Length == 0)
            {
                _logger.LogTrace("Batch {BatchNum} size is 0, skipping", batchCount);
                continue;
            }

            if (validateOnly)
            {
                _logger.LogTrace("Bypassing import batch {BatchNum} (validate mode)", batchCount);
                resultBuilder.MarkBatchSuccessful(batch.Length);
                continue;
            }

            _logger.LogInformation("Importing batch {BatchNum}", batchCount);

            try
            {
                await job.SaveAsync(batch, cancellationToken);
                resultBuilder.MarkBatchSuccessful(batch.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing batch {BatchNum}, halting import.", batchCount);
                resultBuilder.SetStatus(DataImportJobStatus.FatalError);
                break;
            }
        }

        var results = resultBuilder.Build();
        _logger.LogInformation(
            "Import finished with status '{Status}'. Mode: {Mode}, Success: {NumSuccess}, Invalid: {NumInvalid}, Skipped: {NumSkipped}",
            results.Status,
            modeDescription,
            results.NumSuccess,
            results.NumInvalid,
            results.NumSkipped
            );

        return results;
    }
}
