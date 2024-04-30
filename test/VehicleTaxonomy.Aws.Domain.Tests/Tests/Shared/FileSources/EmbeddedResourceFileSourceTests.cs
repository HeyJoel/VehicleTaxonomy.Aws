// Adapted from Cofoundry under MIT Licence
// https://github.com/cofoundry-cms/cofoundry/blob/855a525/test/Cofoundry.Domain.Tests/Domain/Shared/Models/Assets/EmbeddedResourceFileSourceTests.cs

namespace VehicleTaxonomy.Aws.Domain.Tests.Shared.FileSources;

public class EmbeddedResourceFileSourceTests
{
    [Theory]
    [InlineData("Parent.Sub-Dir", "My-Lib.min.js", "Parent.Sub_Dir.My-Lib.min.js")]
    [InlineData(".Parent.Sub-Dir.", ".My-Lib.min.js", "Parent.Sub_Dir..My-Lib.min.js")]
    public void Ctor_FormatsFullPath(string path, string fileName, string expected)
    {
        var assembly = this.GetType().Assembly;
        var instance = new EmbeddedResourceFileSource(assembly, path, fileName);

        Assert.Equal(expected, instance.FullPath);
        Assert.Equal(fileName, instance.FileName);
        Assert.Equal(assembly, instance.Assembly);
    }

    [Fact]
    public async Task OpenReadStreamAsync_WhenExists_OpensStream()
    {
        var assembly = this.GetType().Assembly;
        var instance = new EmbeddedResourceFileSource(assembly, "VehicleTaxonomy.Aws.Domain.Tests.Tests.Shared.FileSources..Test-_Resouce!Directory()[]", ".Test re.source-!.txt");
        using var stream = await instance.OpenReadStreamAsync();

        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public async Task OpenReadStreamAsync_WhenNotExists_Throws()
    {
        var assembly = this.GetType().Assembly;
        var instance = new EmbeddedResourceFileSource(assembly, "VehicleTaxonomy.Aws.Domain.Tests.Tests.Shared.FileSources", "NotFound.txt");

        await instance
            .Awaiting(f => f.OpenReadStreamAsync())
            .Should()
            .ThrowAsync<FileNotFoundException>();
    }
}
