using System.Globalization;
using nietras.SeparatedValues;
using VehicleTaxonomy.Aws.Domain.Variants;
using VehicleTaxonomy.Aws.Infrastructure.DataImport;
using VehicleTaxonomy.Aws.Infrastructure.Db;

namespace VehicleTaxonomy.Aws.Domain.DataImport;

public class TaxonomyFromCsvImportJob : ICsvDataImportJob<TaxonomyCsvRow>
{
    private readonly TaxonomyCsvRowValidator _rowValidator = new();
    private readonly TimeProvider _timeProvider;
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public TaxonomyFromCsvImportJob(
        TimeProvider timeProvider,
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _timeProvider = timeProvider;
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public int? BatchSize => 1000;

    public TaxonomyCsvRow? Map(IDataImportResultBuilder resultBuilder, SepReader.Row row)
    {
        if (row["BodyType"].Span.CompareTo("Cars", StringComparison.OrdinalIgnoreCase) != 0)
        {
            resultBuilder.MarkSkipped(row.RowIndex, "Invalid body type");
            return null;
        }

        if (IsFieldEmpty(row["Make"]))
        {
            const string MAKE_EMPTY_REASON = "Make is empty";
            resultBuilder.MarkSkipped(row.RowIndex, MAKE_EMPTY_REASON);
            return null;
        }

        if (IsFieldEmpty(row["GenModel"]))
        {
            const string GEN_MODEL_EMPTY_REASON = "GenModel is empty";
            resultBuilder.MarkSkipped(row.RowIndex, GEN_MODEL_EMPTY_REASON);
            return null;
        }

        if (IsFieldEmpty(row["Model"]))
        {
            const string MODEL_EMPTY_REASON = "Model is empty";
            resultBuilder.MarkSkipped(row.RowIndex, MODEL_EMPTY_REASON);
            return null;
        }

        var engineSize = row["EngineSizeSimple"].TryParse<int>();
        var fuelCategory = ParseFuelCategory(row["Fuel"].Span);

        var mappedRow = new TaxonomyCsvRow()
        {
            MakeName = row["Make"].ToString(),
            ModelName = row["GenModel"].ToString(),
            VariantName = FormatVariantName(row["Model"].Span, fuelCategory, engineSize),
            FuelCategory = ParseFuelCategory(row["Fuel"].Span),
            EngineSizeInCC = engineSize
        };
        mappedRow.MakeId = EntityIdFormatter.Format(mappedRow.MakeName);
        mappedRow.ModelId = EntityIdFormatter.Format(mappedRow.ModelName);
        mappedRow.VariantId = EntityIdFormatter.Format(mappedRow.VariantName);

        var validationResult = _rowValidator.Validate(mappedRow);
        if (validationResult.IsValid)
        {
            return mappedRow;
        }

        var errors = validationResult
            .Errors
            .Select(e => e.ErrorMessage)
            .ToArray();

        resultBuilder.MarkInvalid(row.RowIndex, errors);

        return null;
    }

    public async Task SaveAsync(IEnumerable<TaxonomyCsvRow> batch, CancellationToken cancellationToken)
    {
        var makes = batch
            .GroupBy(r => r.MakeId, (k, v) => v.First())
            .Select(g => new VehicleTaxonomyDocument()
            {
                CreateDate = _timeProvider.GetUtcNow().DateTime,
                EntityType = VehicleTaxonomyEntity.Make,
                Id = g.MakeId,
                Name = g.MakeName.Trim()
            });

        var models = batch
            .Select(r => new
            {
                Row = r,
                r.MakeId,
                r.ModelId
            })
            .GroupBy(r => $"{r.MakeId}:{r.ModelId}", (k, v) => v.First())
            .Select(g => new VehicleTaxonomyDocument()
            {
                CreateDate = _timeProvider.GetUtcNow().DateTime,
                EntityType = VehicleTaxonomyEntity.Model,
                Id = g.ModelId,
                ParentMakeId = g.MakeId,
                Name = g.Row.ModelName.Trim()
            });

        var variants = batch
            .Select(r => new
            {
                Row = r,
                r.MakeId,
                r.ModelId,
                r.VariantId
            })
            .GroupBy(r => $"{r.MakeId}:{r.ModelId}:{r.VariantId}", (k, v) => v.First())
            .Select(g => new VehicleTaxonomyDocument()
            {
                CreateDate = _timeProvider.GetUtcNow().DateTime,
                EntityType = VehicleTaxonomyEntity.Variant,
                Id = g.VariantId,
                ParentMakeId = g.MakeId,
                ParentModelId = g.ModelId,
                Name = g.Row.VariantName,
                VariantData = new()
                {
                    EngineSizeInCC = g.Row.EngineSizeInCC,
                    FuelCategory = g.Row.FuelCategory?.ToString()
                }
            });

        var documentBatch = makes
            .Concat(models)
            .Concat(variants);

        await _vehicleTaxonomyRepository.AddBatchAsync(documentBatch, cancellationToken);
    }

    private static string FormatVariantName(ReadOnlySpan<char> modelName, FuelCategory? fuelCategory, int? engineSize)
    {
        if (modelName.IsEmpty || modelName.IsWhiteSpace())
        {
            return string.Empty;
        }

        var formatted = modelName.Trim();

        if (engineSize.HasValue)
        {
            var formattedEngineSize = engineSize.Value switch
            {
                < 1 => null,
                >= 1000 => $"{((double)engineSize / 1000).ToString("F1", CultureInfo.InvariantCulture)}l",
                _ => $"{engineSize}cc"
            };

            if (formattedEngineSize != null)
            {
                formatted = $"{formatted} {formattedEngineSize}";
            }
        }

        if (fuelCategory != null && fuelCategory != FuelCategory.Other)
        {
            var formattedFuelCategory = fuelCategory switch
            {
                FuelCategory.ElectricHybridDiesel => "Hybrid Diesel",
                FuelCategory.ElectricHybridPetrol => "Hybrid Petrol",
                _ => fuelCategory.ToString()
            };
            return $"{formatted} {formattedFuelCategory}";
        }

        return formatted.ToString();
    }

    private static FuelCategory? ParseFuelCategory(ReadOnlySpan<char> csvValue)
    {
        if (csvValue.IsEmpty || csvValue.IsWhiteSpace())
        {
            return null;
        }

        var fuelCategory = csvValue switch
        {
            "Battery electric" => FuelCategory.Electric,
            "Diesel" => FuelCategory.Electric,
            "Fuel cell electric" => FuelCategory.Electric,
            "Hybrid electric (Diesel)" => FuelCategory.ElectricHybridDiesel,
            "Hybrid electric (Petrol)" => FuelCategory.ElectricHybridPetrol,
            "Petrol" => FuelCategory.Petrol,
            "Plug-in hybrid electric (Diesel)" => FuelCategory.ElectricHybridDiesel,
            "Plug-in hybrid electric (Petrol)" => FuelCategory.ElectricHybridPetrol,
            "Range extended electric" => FuelCategory.Electric,
            _ => FuelCategory.Other
        };

        return fuelCategory;
    }

    private static bool IsFieldEmpty(SepReader.Col col)
    {
        var value = col.Span;
        return value.IsEmpty
            || value.IsWhiteSpace()
            || value.Equals("MISSING", StringComparison.OrdinalIgnoreCase);
    }
}
