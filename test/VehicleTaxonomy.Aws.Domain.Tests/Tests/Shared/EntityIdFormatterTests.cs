namespace VehicleTaxonomy.Aws.Domain.Tests;

public class EntityIdFormatterTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void Format_WhenNullOrWhitespace_ReturnsEmptyString(string? s)
    {
        var result = EntityIdFormatter.Format(s!);

        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("volkswagen")]
    [InlineData("nissan-sunny")]
    [InlineData("bmw-3-series-3-1")]
    public void Format_WhenSlug_ReturnsSame(string s)
    {
        var result = EntityIdFormatter.Format(s);

        result.Should().Be(s);
    }

    [Theory]
    [InlineData("Crème brûlée", "crme-brle")]
    [InlineData("diakrī́nō", "diakrn")]
    [InlineData("Что-то хорошее", "")]
    public void Format_DiacriticsNotSupported(string input, string expected)
    {
        var result = EntityIdFormatter.Format(input);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("1.2.3", "1-2-3")]
    [InlineData("1,2,3", "1-2-3")]
    [InlineData("en–dash", "en-dash")]
    [InlineData("em—dash", "em-dash")]
    [InlineData("en––double––dash", "en-double-dash")]
    [InlineData(".dots....ahoy!", "dots-ahoy")]
    [InlineData("underscore_between_words", "underscore-between-words")]
    [InlineData("1:30", "1-30")]
    [InlineData("2+2=5", "2-2-5")]
    [InlineData("this/that", "this-that")]
    [InlineData("this\\that", "this-that")]
    public void Format_WithSeparatorPunctuation_ConvertsToDash(string input, string expected)
    {
        var result = EntityIdFormatter.Format(input);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(" with a space at start", "with-a-space-at-start")]
    [InlineData("with a space at end ", "with-a-space-at-end")]
    [InlineData("   with space at start", "with-space-at-start")]
    [InlineData("with space at end   ", "with-space-at-end")]
    [InlineData("with puntucation at end+=.–:", "with-puntucation-at-end")]
    [InlineData("+=.–:with puntucation at start", "with-puntucation-at-start")]
    public void Format_TrimsExcessNonCharacters(string input, string expected)
    {
        var result = EntityIdFormatter.Format(input);

        result.Should().Be(expected);
    }
}
