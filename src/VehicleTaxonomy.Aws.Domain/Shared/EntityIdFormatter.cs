using System.Text.RegularExpressions;

namespace VehicleTaxonomy.Aws.Domain;

public partial class EntityIdFormatter
{
    /// <summary>
    /// Converts an entity name into a url-friendly identifier using
    /// "slug" formatting i.e. lowercase a-z, 0-9 and dashes. For example
    /// "VW Polo" would become "vw-polo". May return an empty string if no
    /// valid characters could be found.
    /// </summary>
    public static string Format(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var str = name.ToLowerInvariant();
        str = SignificantPunctuationRegex().Replace(str, " ");
        str = NotSlugCharsRegex().Replace(str, "");
        str = WhitespaceRegex().Replace(str, "-");
        str = MultiDashRegex().Replace(str, "-");

        return str.Trim('-');
    }

    [GeneratedRegex(@"[/\\\.,\+=–—:_]")]
    private static partial Regex SignificantPunctuationRegex();

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NotSlugCharsRegex();

    [GeneratedRegex(@"\s")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"--+")]
    private static partial Regex MultiDashRegex();
}
