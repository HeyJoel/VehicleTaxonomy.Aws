using System.Text;
using FluentAssertions.Execution;
using nietras.SeparatedValues;
using VehicleTaxonomy.Aws.Infrastructure.DataImport;

namespace VehicleTaxonomy.Aws.Infrastructure.Tests.DataImport;

public class CsvDataImportJobRunnerTests
{
    private readonly ILogger<CsvDataImportJobRunner> _logger = NullLogger<CsvDataImportJobRunner>.Instance;

    [Fact]
    public async Task Run_WhenEmptyCsv_NoAction()
    {
        using var stream = CreateCsv("test", string.Empty);
        var count = 0;

        var job = new MockJob()
        {
            OnMap = builder => { count++; return null; },
            OnSave = batch => count++,
        };

        var jobRunner = new CsvDataImportJobRunner(_logger);
        var result = await jobRunner.RunAsync(stream, job);

        count.Should().Be(0);
        InlineSnapshot.Validate(result, """
            Status: Finished
            SkippedReasons: {}
            ValidationErrors: {}
            """);
    }

    [Fact]
    public async Task Run_NullResultsNotSaved()
    {
        using var stream = CreateCsv("test", "0", "1", "2");
        var mapCount = 0;
        var saveCount = 0;

        var job = new MockJob()
        {
            OnMap = builder =>
            {
                mapCount++;
                return mapCount % 2 == 0 ? null : new();
            },
            OnSave = batch => saveCount += batch.Count(),
        };

        var jobRunner = new CsvDataImportJobRunner(_logger);
        var result = await jobRunner.RunAsync(stream, job);

        using (new AssertionScope())
        {
            mapCount.Should().Be(3);
            saveCount.Should().Be(2);
            InlineSnapshot.Validate(result, """
                NumSuccess: 2
                Status: Finished
                SkippedReasons: {}
                ValidationErrors: {}
                """);
        }
    }

    [Fact]
    public async Task Run_CanSetResult()
    {
        using var stream = CreateCsv("test", "0", "1", "2", "3", "4");
        var mapCount = 0;

        var job = new MockJob()
        {
            OnMap = builder =>
            {
                if (mapCount % 2 == 0)
                {
                    builder.MarkSkipped(mapCount, "test reason");
                }
                else
                {
                    builder.MarkInvalid(mapCount, ["test error"]);
                }
                mapCount++;
                return null;
            }
        };

        var jobRunner = new CsvDataImportJobRunner(_logger);
        var result = await jobRunner.RunAsync(stream, job);

        using (new AssertionScope())
        {
            InlineSnapshot.Validate(result, """
                NumSkipped: 3
                NumInvalid: 2
                Status: Finished
                SkippedReasons:
                  test reason:
                    - 0
                    - 2
                    - 4
                ValidationErrors:
                  test error:
                    - 1
                    - 3
                """);
        }
    }

    [Fact]
    public async Task Run_WhenExceptionOnSave_HaltsProcess()
    {
        using var stream = CreateCsv("test", "0", "1", "2", "3", "4");
        var mapCount = 0;

        var job = new MockJob()
        {
            OnMap = builder => new(),
            OnSave = batch =>
            {
                mapCount++;
                if (mapCount == 3)
                {
                    throw new Exception("Example exception");
                }
            },
            BatchSize = 1
        };

        var jobRunner = new CsvDataImportJobRunner(_logger);
        var result = await jobRunner.RunAsync(stream, job);

        using (new AssertionScope())
        {
            InlineSnapshot.Validate(result, """
                NumSuccess: 2
                Status: FatalError
                SkippedReasons: {}
                ValidationErrors: {}
                """);
        }
    }

    [Fact]
    public async Task Run_CanBatch()
    {
        using var stream = CreateCsv("test", "0", "1", "2", "3", "4", "5", "6");
        var maxBatchSize = 0;
        var batchCount = 0;

        var job = new MockJob()
        {
            OnMap = builder => new(),
            OnSave = batch =>
            {
                batchCount++;
                var batchSize = batch.Count();
                if (batchSize > maxBatchSize)
                {
                    maxBatchSize = batchSize;
                }
            },
            BatchSize = 2
        };

        var jobRunner = new CsvDataImportJobRunner(_logger);
        var result = await jobRunner.RunAsync(stream, job);

        using (new AssertionScope())
        {
            maxBatchSize.Should().Be(2);
            batchCount.Should().Be(4);
            InlineSnapshot.Validate(result, """
                NumSuccess: 7
                Status: Finished
                SkippedReasons: {}
                ValidationErrors: {}
                """);
        }
    }

    private static MemoryStream CreateCsv(string headers, params string[] rows)
    {
        var csv = $"{headers}{Environment.NewLine}{string.Join(Environment.NewLine, rows)}";
        var bytes = Encoding.UTF8.GetBytes(csv);
        return new MemoryStream(bytes);
    }

    private record EmptyCsvRow();

    private class MockJob : ICsvDataImportJob<EmptyCsvRow>
    {
        public int? BatchSize { get; set; }

        public Func<IDataImportResultBuilder, EmptyCsvRow?>? OnMap { get; set; }

        public Action<IEnumerable<EmptyCsvRow>>? OnSave { get; set; }

        public EmptyCsvRow? Map(IDataImportResultBuilder resultBuilder, SepReader.Row row)
        {
            var result = OnMap?.Invoke(resultBuilder);
            return result;
        }

        public Task SaveAsync(IEnumerable<EmptyCsvRow> batch, CancellationToken cancellationToken)
        {
            OnSave?.Invoke(batch);

            return Task.CompletedTask;
        }
    }
}
