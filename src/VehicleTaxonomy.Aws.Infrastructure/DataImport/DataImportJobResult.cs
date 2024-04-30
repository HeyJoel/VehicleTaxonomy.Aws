using System.Text.Json.Serialization;

namespace VehicleTaxonomy.Aws.Infrastructure.DataImport;

public class DataImportJobResult
{
    /// <summary>
    /// The number of input records (e.g. CSV rows) processed
    /// successfully.
    /// </summary>
    public int NumSuccess { get; set; }

    /// <summary>
    /// The number of input records (e.g. CSV rows) skipped
    /// e.g. empty or irrelevant (but not necessarily invalid).
    /// </summary>
    public int NumSkipped { get; set; }

    /// <summary>
    /// The number of records not processed because they failed
    /// validation.
    /// </summary>
    public int NumInvalid { get; set; }

    /// <summary>
    /// The current status of the import job. Typically this will
    /// simply be either <see cref="DataImportJobStatus.Finished"/>
    /// or <see cref="DataImportJobStatus.FatalError"/> at the end
    /// of the job run.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<DataImportJobStatus>))]
    public DataImportJobStatus Status { get; set; }

    /// <summary>
    /// Information on skipped records. The key is the user displayable
    /// "reason" text and the value is a set of record (e.g. CSV row)
    /// indexes of the records that were skipped for that reason.
    /// </summary>
    public IDictionary<string, ISet<int>> SkippedReasons { get; set; } = new Dictionary<string, ISet<int>>();

    /// <summary>
    /// Information on invalid records. The key is the user displayable
    /// validation error text and the value is a set of record (e.g. CSV row)
    /// indexes of the records that were invalid due to the error.
    /// </summary>
    public IDictionary<string, ISet<int>> ValidationErrors { get; set; } = new Dictionary<string, ISet<int>>();
}
