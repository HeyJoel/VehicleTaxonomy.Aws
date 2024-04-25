namespace VehicleTaxonomy.Aws.Infrastructure.Db;

public class DynamoDbOptions
{
    /// <summary>
    /// Configuration section name in an appsettings.json
    /// file or similar.
    /// </summary>
    public const string SectionName = "DynamoDb";

    /// <summary>
    /// When <see langword="true"/>, the application will attempt
    /// to connect to a local test instance of DynamoDb using the
    /// specified <see cref="LocalDbServiceUrl"/>.
    /// </summary>
    public bool UseLocalDb { get; set; }

    /// <summary>
    /// The url to use when connecting to a local DynamoDb instance.
    /// Defaults to "http://localhost:8000", which is the default
    /// binding for a docker instance.
    /// </summary>
    public string LocalDbServiceUrl { get; set; } = "http://localhost:8000";
}
