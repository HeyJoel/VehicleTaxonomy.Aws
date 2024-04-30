using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Aws.Domain.DataImport;
using VehicleTaxonomy.Aws.Domain.Tests.Makes;
using VehicleTaxonomy.Aws.Domain.Tests.Models;
using VehicleTaxonomy.Aws.Domain.Tests.Variants;

namespace VehicleTaxonomy.Aws.Domain.Tests.DataImport;

[Collection(nameof(DbDependentFixtureCollection))]
public class ImportTaxonomyFromCsvCommandHandlerTests
{
    private readonly DbDependentFixture _dbDependentFixture;

    public ImportTaxonomyFromCsvCommandHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValidSingleRow_CanImport()
    {
        var makeId = "imptaxcsvch-valsrow";
        var modelId = "abarth-124";
        var variantId = "124-gt-multiair-1-4l-petrol";

        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync(nameof(WhenValidSingleRow_CanImport)),
            ImportMode = DataImportMode.Run
        });

        var dbMake = await makeTestHelper.GetRawRecordAsync(makeId);
        var dbModel = await modelTestHelper.GetRawRecordAsync(makeId, modelId);
        var dbVariant = await variantTestHelper.GetRawRecordAsync(makeId, modelId, variantId);

        response.IsValid.Should().BeTrue();

        InlineSnapshot.Validate(response.Result, """
            NumSuccess: 1
            Status: Finished
            SkippedReasons: {}
            ValidationErrors: {}
            """);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbMake, """
                SK:
                  S: imptaxcsvch-valsrow
                PK:
                  S: make#imptaxcsvch-valsrow
                CreateDate:
                  S: 2024-04-24T12:56:33.000Z
                Name:
                  S: ImpTaxCsvCH ValSRow
                MakeId:
                  S: imptaxcsvch-valsrow
                """);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbModel, """
                SK:
                  S: abarth-124
                PK:
                  S: make#imptaxcsvch-valsrow#models
                CreateDate:
                  S: 2024-04-24T12:56:33.000Z
                Name:
                  S: ABARTH 124
                """);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbVariant, """
                SK:
                  S: 124-gt-multiair-1-4l-petrol
                PK:
                  S: make#imptaxcsvch-valsrow#model#abarth-124#variants
                CreateDate:
                  S: 2024-04-24T12:56:33.000Z
                Name:
                  S: 124 GT MULTIAIR 1.4l Petrol
                """);
    }

    [Fact]
    public async Task WhenValidMultiRow_CanImport()
    {
        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync(nameof(WhenValidMultiRow_CanImport)),
            ImportMode = DataImportMode.Run
        });

        response.IsValid.Should().BeTrue();
        InlineSnapshot.Validate(response.Result, """
            NumSuccess: 51
            NumSkipped: 5
            NumInvalid: 2
            Status: Finished
            SkippedReasons:
              Model is empty:
                - 4
              Invalid body type:
                - 5
                - 6
                - 9
                - 10
            ValidationErrors:
              The length of 'Model Name' must be 50 characters or fewer:
                - 53
              The length of 'Make Name' must be 50 characters or fewer:
                - 54
            """);
    }

    [Fact]
    public async Task WhenModeValidate_DoesNotImport()
    {
        var makeId = "imptaxcsvch-modeval-notimp";

        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync(nameof(WhenModeValidate_DoesNotImport)),
            ImportMode = DataImportMode.Validate
        });

        var dbMake = await makeTestHelper.GetRawRecordAsync(makeId);

        using (new AssertionScope())
        {
            dbMake.Should().BeNull();
            response.IsValid.Should().BeTrue();
            InlineSnapshot.Validate(response.Result, """
                Status: Finished
                SkippedReasons: {}
                ValidationErrors: {}
                """);
        }
    }

    [Fact]
    public async Task WhenInvalidFile_ReturnsError()
    {
        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync("BadFile"),
            ImportMode = DataImportMode.Run
        });

        using (new AssertionScope())
        {
            response.IsValid.Should().BeFalse();
            response.ValidationErrors.Should().HaveCount(1);
            var error = response.ValidationErrors.First();
            error.Property.Should().Be(nameof(ImportTaxonomyFromCsvCommand.File));
            response.Result.Should().BeNull();
        }
    }

    private EmbeddedResourceFileSource GetTestFileSourceAsync(string uniqueName)
    {
        var fileSource = new EmbeddedResourceFileSource(
            GetType().Assembly,
            "VehicleTaxonomy.Aws.Domain.Tests.Tests.DataImport.TestResources",
            $"{nameof(ImportTaxonomyFromCsvCommandHandlerTests)}_{uniqueName}.csv"
            );

        return fileSource;
    }
}
