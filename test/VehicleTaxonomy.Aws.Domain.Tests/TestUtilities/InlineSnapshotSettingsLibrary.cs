using Meziantou.Framework.HumanReadable;
using Meziantou.Framework.InlineSnapshotTesting;
using Meziantou.Framework.InlineSnapshotTesting.Serialization;

namespace VehicleTaxonomy.Aws.Domain.Tests;

/// <summary>
/// For more info on snapshot testing see 
/// <see href="https://www.meziantou.net/inline-snapshot-testing-in-dotnet.htm"/>
/// </summary>
public class InlineSnapshotSettingsLibrary
{
    /// <summary>
    /// Removes default values and empty collections from the output which
    /// can be verbose for the DynamoDb get response.
    /// </summary>
    public static InlineSnapshotSettings IgnoreDefaultOrEmptyCollection = InlineSnapshotSettings.Default with
    {
        SnapshotSerializer = new HumanReadableSnapshotSerializer(s => s.DefaultIgnoreCondition = HumanReadableIgnoreCondition.WhenWritingDefaultOrEmptyCollection)
    };
}
