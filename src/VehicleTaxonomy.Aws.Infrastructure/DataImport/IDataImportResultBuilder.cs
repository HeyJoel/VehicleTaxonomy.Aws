namespace VehicleTaxonomy.Aws.Infrastructure.DataImport;

/// <summary>
/// Used to incrementally update the result of an import job while
/// the process is running.
/// </summary>
public interface IDataImportResultBuilder
{
    /// <summary>
    /// Marks the record at the specified <paramref name="recordIndex"/> as
    /// invalid.
    /// </summary>
    /// <param name="recordIndex">
    /// The index of the record in the import source data e.g. the CSV row.
    /// </param>
    /// <param name="validationErrorMessages">
    /// One or more user displayable messages describing why the record is invalid.
    /// The messages should not be record-specifc, as this will prevent errors from being
    /// aggregated and bloat the size of the result.
    /// </param>
    void MarkInvalid(int recordIndex, IReadOnlyCollection<string> validationErrorMessages);

    /// <summary>
    /// Marks the record at the specified <paramref name="recordIndex"/> as
    /// skipped e.g. if the record was empty or irrelevant but not necessarily invalid.
    /// </summary>
    /// <param name="recordIndex">
    /// The index of the record in the import source data e.g. the CSV row.
    /// </param>
    /// <param name="reason">
    /// A user displayable message describing why the record is skipped. The reason
    /// text should not be record-specifc, as this will prevent errors from being
    /// aggregated and bloat the size of the result.
    /// </param>
    void MarkSkipped(int recordIndex, string reason);

    /// <summary>
    /// Marks a batch of results as successful.
    /// </summary>
    /// <param name="numSuccessful">
    ///The number of records in the successful batch.
    /// </param>
    void MarkBatchSuccessful(int numSuccessful);
}
