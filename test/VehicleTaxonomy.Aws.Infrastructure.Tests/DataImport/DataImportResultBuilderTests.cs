using VehicleTaxonomy.Aws.Infrastructure.DataImport;

namespace VehicleTaxonomy.Aws.Infrastructure.Tests.DataImport;

public class DataImportResultBuilderTests
{
    private readonly ILogger<CsvDataImportJobRunner> _logger = NullLogger<CsvDataImportJobRunner>.Instance;

    [Fact]
    public void CanMarkInvalid()
    {
        const string errorMessage1 = "test message 1";
        const string errorMessage2 = "test message 2";

        var builder = new DataImportResultBuilder(_logger);
        builder.MarkInvalid(23, [errorMessage1, errorMessage2]);
        builder.MarkInvalid(45, [errorMessage1]);

        var result = builder.Build();

        InlineSnapshot.Validate(result, """
            NumInvalid: 2
            Status: Finished
            SkippedReasons: {}
            ValidationErrors:
              test message 1:
                - 23
                - 45
              test message 2:
                - 23
            """);
    }

    [Fact]
    public void CanMarkSkipped()
    {
        const string skipReason1 = "test reason 1";
        const string skipReason2 = "test reason 2";

        var builder = new DataImportResultBuilder(_logger);
        builder.MarkSkipped(42, skipReason1);
        builder.MarkSkipped(50263, skipReason2);
        builder.MarkSkipped(1053, skipReason1);

        var result = builder.Build();

        InlineSnapshot.Validate(result, """
            NumSkipped: 3
            Status: Finished
            SkippedReasons:
              test reason 1:
                - 42
                - 1053
              test reason 2:
                - 50263
            ValidationErrors: {}
            """);
    }

    [Fact]
    public void CanMarkSuccess()
    {
        var builder = new DataImportResultBuilder(_logger);
        builder.MarkBatchSuccessful(1200);
        builder.MarkBatchSuccessful(1);

        var result = builder.Build();

        InlineSnapshot.Validate(result, """
            NumSuccess: 1201
            Status: Finished
            SkippedReasons: {}
            ValidationErrors: {}
            """);
    }

    [Fact]
    public void WhenInvalidLimit_Enforced()
    {
        const string errorMessage = "test message";

        var builder = new DataImportResultBuilder(_logger)
        {
            InvalidRecordDetailsLimit = 2
        };

        builder.MarkInvalid(23, [errorMessage]);
        builder.MarkInvalid(45, [errorMessage]);
        builder.MarkInvalid(63, [errorMessage]);
        builder.MarkInvalid(79, [errorMessage]);

        var result = builder.Build();

        InlineSnapshot.Validate(result, """
            NumInvalid: 4
            Status: Finished
            SkippedReasons: {}
            ValidationErrors:
              test message:
                - 23
                - 45
            """);
    }

    [Fact]
    public void WhenSkippedLimit_Enforced()
    {
        const string skipedReason = "test reason";

        var builder = new DataImportResultBuilder(_logger)
        {
            SkippedRecordDetailsLimit = 2
        };
        builder.MarkSkipped(23, skipedReason);
        builder.MarkSkipped(45, skipedReason);
        builder.MarkSkipped(63, skipedReason);
        builder.MarkSkipped(79, skipedReason);

        var result = builder.Build();

        InlineSnapshot.Validate(result, """
            NumSkipped: 4
            Status: Finished
            SkippedReasons:
              test reason:
                - 23
                - 45
            ValidationErrors: {}
            """);
    }

    [Fact]
    public void CanSetStatus()
    {
        var builder = new DataImportResultBuilder(_logger);
        builder.SetStatus(DataImportJobStatus.FatalError);

        var result = builder.Build();

        result.Status.Should().Be(DataImportJobStatus.FatalError);
    }
}
