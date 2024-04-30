namespace VehicleTaxonomy.Aws.Infrastructure.DataImport;

public enum DataImportJobStatus
{
    /// <summary>
    /// The job has not yet started to process.
    /// </summary>
    NoStarted,

    /// <summary>
    /// The job has started processing.
    /// </summary>
    Processing,

    /// <summary>
    /// The job has finished without any fatal errors.
    /// </summary>
    Finished,

    /// <summary>
    /// The job encountered a fatal error that prevented further processing.
    /// </summary>
    FatalError
}
