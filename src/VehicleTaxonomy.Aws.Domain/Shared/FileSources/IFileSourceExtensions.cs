namespace VehicleTaxonomy.Aws.Domain.Shared.FileSources;

public static class IFileSourceExtensions
{
    /// <summary>
    /// Attempts to opens a stream of the file contents, returning <see langword="null"/>
    /// if an exception was thrown. The callee is responsible for disposing 
    /// of the stream.
    /// </summary>
    /// <param name="fileSource">
    /// File reference to open.
    /// </param>
    /// <param name="exceptionLogger">
    /// An optional logger to log exception data out to.
    /// </param>
    public static async Task<Stream?> TryOpenReadStreamAsync(this IFileSource fileSource, ILogger? exceptionLogger = null)
    {
        Stream? stream = null;

        try
        {
            stream = await fileSource.OpenReadStreamAsync();
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            exceptionLogger?.LogError(fileNotFoundException, "File '{FileName}' could not be found.", fileSource.FileName);
        }
        catch (Exception exception)
        {
            exceptionLogger?.LogError(exception, "Error opening file {0}", fileSource.FileName);
        }

        return stream;
    }
}
