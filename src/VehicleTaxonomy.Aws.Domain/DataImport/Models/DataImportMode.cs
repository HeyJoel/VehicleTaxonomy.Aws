namespace VehicleTaxonomy.Aws.Domain.DataImport;

public enum DataImportMode
{
    /// <summary>
    /// Validate the data import source only. Do not import
    /// any data.
    /// </summary>
    Validate,

    /// <summary>
    /// Validate and run the import. Invalid data will be skipped.
    /// </summary>
    Run
}
