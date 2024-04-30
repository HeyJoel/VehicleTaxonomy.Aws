using VehicleTaxonomy.Aws.Domain.Variants;

namespace VehicleTaxonomy.Aws.Domain.DataImport;

public class TaxonomyCsvRow
{
    public string MakeId { get; set; } = string.Empty;

    public string MakeName { get; set; } = string.Empty;

    public string ModelId { get; set; } = string.Empty;

    public string ModelName { get; set; } = string.Empty;

    public string VariantId { get; set; } = string.Empty;

    public string VariantName { get; set; } = string.Empty;

    public int? EngineSizeInCC { get; set; }

    public FuelCategory? FuelCategory { get; set; }
}
