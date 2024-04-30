using nietras.SeparatedValues;

namespace VehicleTaxonomy.Aws.Infrastructure.DataImport;

/// <summary>
/// The custom implementation parts of a CSV data import (ETL) process. The job
/// is passed into the <see cref="CsvDataImportJobRunner"/> which handles the
/// standard functionality to load a CSV, batch up data and run the <see cref="Map"/>
/// and <see cref="SaveAsync"/> stages of the process.
/// </summary>
/// <typeparam name="TMappedRecord">
/// The type of object output from the mapping (transform) stage of the
/// import process.
/// </typeparam>
public interface ICsvDataImportJob<TMappedRecord>
{
    /// <summary>
    /// Optionally chunk the data read from the CSV into batches. Each batch will be
    /// run through the <see cref="Map"/> and <see cref="SaveAsync"/> process before
    /// moving onto the next batch.
    /// </summary>
    int? BatchSize { get; }

    /// <summary>
    /// Maps (transforms) the CSV row data into the <see cref="TMappedRecord"/>
    /// that is eventually passed to <see cref="SaveAsync"/>. The data should be
    /// validated at this stage, returning <see langword="null"/> if the result is
    /// invalid or should be skipped. Use the <paramref name="resultBuilder"/> to
    /// log when data is invalid or skipped.
    /// </summary>
    /// <param name="resultBuilder">
    /// Use the result builder to log when data is invalid or skipped. This will
    /// be used to build the result that is passed back at the end of the process.
    /// </param>
    /// <param name="row">
    /// The CSV data row to process. See <see href="https://github.com/nietras/Sep"/>
    /// for info on how to work with <see cref="SepReader.Row"/>. 
    /// </param>
    /// <returns>
    /// If successful the mapped data record should be returned, otherwise
    /// return <see langword="null"/>.
    /// </returns>
    TMappedRecord? Map(IDataImportResultBuilder resultBuilder, SepReader.Row row);

    /// <summary>
    /// Save the batch of mapped records to your data store. If an exception is thrown
    /// then the process will be halted and the error logged before returning a result
    /// of status <see cref="DataImportJobStatus.FatalError"/>.
    /// </summary>
    /// <param name="batch">
    /// The batch of records to process. Batching is dependent on the
    /// configured <see cref="BatchSize"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token to pass on to downstream async services.
    /// </param>
    Task SaveAsync(IEnumerable<TMappedRecord> batch, CancellationToken cancellationToken);
}
