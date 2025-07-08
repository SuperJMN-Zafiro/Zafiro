using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes;

/// <summary>
/// Extension methods for creating ByteSource from URIs
/// These methods provide advanced and flexible APIs
/// </summary>
public static class ByteSourceUriExtensions
{
    /// <summary>
    /// Creates a ByteSource from a URI using an IUriContentProvider
    /// This is the most flexible method that allows dependency injection
    /// </summary>
    /// <param name="uri">The URI from which to obtain the content</param>
    /// <param name="contentProvider">The content provider to handle the download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Result with the ByteSource if the operation is successful</returns>
    public static async Task<Result<IByteSource>> ToByteSourceAsync(
        this Uri uri, 
        IUriContentProvider contentProvider, 
        CancellationToken cancellationToken = default)
    {
        return await contentProvider.GetByteSourceAsync(uri, cancellationToken);
    }

    /// <summary>
    /// Creates a ByteSource from a URI using a provided HttpClient
    /// Extension method that delegates to the main factory methods
    /// </summary>
    /// <param name="uri">The URI from which to obtain the content</param>
    /// <param name="httpClient">The HttpClient to use for the download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Result with the ByteSource if the operation is successful</returns>
    public static async Task<Result<IByteSource>> ToByteSource(
        this Uri uri, 
        HttpClient httpClient, 
        CancellationToken cancellationToken = default)
    {
        return await ByteSourceUriFactoryMethods.FromUri(uri, httpClient, cancellationToken);
    }
}

/// <summary>
/// Extension methods for creating ByteSource from URI strings
/// </summary>
public static class StringUriExtensions
{
    /// <summary>
    /// Creates a ByteSource from a URI string using an IUriContentProvider
    /// </summary>
    /// <param name="uriString">The URI string from which to obtain the content</param>
    /// <param name="contentProvider">The content provider to handle the download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Result with the ByteSource if the operation is successful</returns>
    public static async Task<Result<IByteSource>> ToByteSource(
        this string uriString, 
        IUriContentProvider contentProvider, 
        CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
        {
            return Result.Failure<IByteSource>($"Invalid URI: {uriString}");
        }

        return await uri.ToByteSourceAsync(contentProvider, cancellationToken);
    }

    /// <summary>
    /// Creates a ByteSource from a URI string using a provided HttpClient
    /// Extension method that delegates to the main factory methods
    /// </summary>
    /// <param name="uriString">The URI string from which to obtain the content</param>
    /// <param name="httpClient">The HttpClient to use for the download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Result with the ByteSource if the operation is successful</returns>
    public static async Task<Result<IByteSource>> ToByteSource(
        this string uriString, 
        HttpClient httpClient, 
        CancellationToken cancellationToken = default)
    {
        return await ByteSourceUriFactoryMethods.FromUri(uriString, httpClient, cancellationToken);
    }
}
