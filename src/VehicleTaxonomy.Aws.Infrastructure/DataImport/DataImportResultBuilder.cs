namespace VehicleTaxonomy.Aws.Infrastructure.DataImport;

internal class DataImportResultBuilder : IDataImportResultBuilder
{
    private readonly ILogger _logger;
    private readonly DataImportJobResult _importJobResult = new();

    public DataImportResultBuilder(
        ILogger logger
        )
    {
        _logger = logger;
    }

    /// <summary>
    /// The maximum number of records to include detailed reasons about
    /// why they were skipped. This is used to limit the result size
    /// for large import jobs. If <see langword="null"/> then no
    /// limit is set; if 0 or less then no data is stored.
    /// </summary>
    public int? SkippedRecordDetailsLimit { get; set; } = 500;

    /// <summary>
    /// The maximum number of records to include detailed validation
    /// error messages for. This is used to limit the result size
    /// for large import jobs. If <see langword="null"/> then no
    /// limit is set; if 0 or less then no data is stored.
    /// </summary>
    public int? InvalidRecordDetailsLimit { get; set; } = 500;

    /// <summary>
    /// Sets the status of the import process. You don't need to
    /// mark the process as <see cref="DataImportJobStatus.Finished"/>
    /// as this will be done automatically when you <see cref="Build"/>
    /// is called at the end of the process.
    /// </summary>
    /// <param name="status">
    /// The status value to set.
    /// </param>
    public void SetStatus(DataImportJobStatus status)
    {
        _importJobResult.Status = status;
    }

    /// <inheritdoc/>
    public void MarkInvalid(int rowIndex, IReadOnlyCollection<string> validationErrorMessages)
    {
        _importJobResult.NumInvalid++;

        foreach (var errorErrorMessage in validationErrorMessages)
        {
            _logger.LogDebug("Row {RowIndex} invalid, error: {Error}", rowIndex, errorErrorMessage);
        }
        if (InvalidRecordDetailsLimit.HasValue && _importJobResult.NumInvalid > InvalidRecordDetailsLimit)
        {
            return;
        }

        foreach (var message in validationErrorMessages)
        {
            if (_importJobResult.ValidationErrors.TryGetValue(message, out var rowIndexes))
            {
                rowIndexes.Add(rowIndex);
            }
            else
            {
                _importJobResult.ValidationErrors[message] = new HashSet<int>() { rowIndex };
            }
        }
    }

    /// <inheritdoc/>
    public void MarkSkipped(int recordIndex, string reason)
    {
        _importJobResult.NumSkipped++;
        _logger.LogDebug("Row {RowIndex} skipped, reason: {Reason}", recordIndex, reason);

        if (_importJobResult.NumSkipped > SkippedRecordDetailsLimit)
        {
            return;
        }
        if (_importJobResult.SkippedReasons.TryGetValue(reason, out var rowIndexes))
        {
            rowIndexes.Add(recordIndex);
        }
        else
        {
            _importJobResult.SkippedReasons[reason] = new HashSet<int>() { recordIndex };
        }
    }

    /// <inheritdoc/>
    public void MarkBatchSuccessful(int numSuccessful)
    {
        _importJobResult.NumSuccess += numSuccessful;
    }

    /// <summary>
    /// Returns the completed <see cref="DataImportJobResult"/>, marking it
    /// as <see cref="DataImportJobStatus.Finished"/> if the job hasn't already
    /// been marked as <see cref="DataImportJobStatus.FatalError"/>.
    /// </summary>
    /// <returns></returns>
    public DataImportJobResult Build()
    {
        if (_importJobResult.Status != DataImportJobStatus.FatalError)
        {
            _importJobResult.Status = DataImportJobStatus.Finished;
        }
        return _importJobResult;
    }
}
