using System.Reactive.Linq;
using System.Text;
using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes;

/// <summary>
/// A reactive source of byte arrays that also provides an optional asynchronous way to retrieve its length.
/// </summary>
public class ByteSource(IObservable<byte[]> bytes, Func<Task<Maybe<long>>>? getLength = null) : IByteSource
{
    /// <summary>
    /// Exposes the underlying observable of byte[] blocks.
    /// </summary>
    public IObservable<byte[]> Bytes => bytes;

    /// <summary>
    /// Asynchronously retrieves the length of this source if it is known; otherwise returns None.
    /// </summary>
    public Task<Maybe<long>> GetLength() => getLength?.Invoke() ?? Task.FromResult(Maybe<long>.None);

    /// <summary>
    /// Constructor overload that accepts an observable of byte chunks (IEnumerable<byte>) 
    /// and transforms each chunk into a byte array internally.
    /// </summary>
    public ByteSource(IObservable<IEnumerable<byte>> byteChunks, Func<Task<Maybe<long>>>? getLength = null)
        : this(byteChunks.Select(x => x.ToArray()), getLength)
    {
    }

    /// <summary>
    /// Creates a ByteSource from a byte array. The data is split into chunks of the specified bufferSize.
    /// The length is automatically taken as bytes.Length.
    /// </summary>
    /// <param name="bytes">Source array of bytes.</param>
    /// <param name="bufferSize">Size of each emitted chunk.</param>
    /// <returns>An IByteSource.</returns>
    public static IByteSource FromBytes(byte[] bytes, int bufferSize = 4096)
    {
        return new ByteSource(
            bytes.ToByteStream(bufferSize),
            () => Task.FromResult<Maybe<long>>(bytes.Length)
        );
    }

    /// <summary>
    /// Creates a ByteSource from an observable of byte chunks (IEnumerable<byte>).
    /// </summary>
    /// <param name="byteChunks">Observable sequence where each item is a chunk of bytes (IEnumerable&lt;byte&gt;).</param>
    /// <param name="getLength">A function that can asynchronously provide the total length if known.</param>
    /// <returns>An IByteSource.</returns>
    public static IByteSource FromByteChunks(
        IObservable<IEnumerable<byte>> byteChunks,
        Func<Task<Maybe<long>>>? getLength = null)
    {
        return new ByteSource(byteChunks, getLength);
    }

    /// <summary>
    /// Creates a ByteSource from an observable of byte arrays.
    /// </summary>
    /// <param name="byteObservable">Observable sequence where each item is an array of bytes.</param>
    /// <param name="getLength">A function that can asynchronously provide the total length if known.</param>
    /// <returns>An IByteSource.</returns>
    public static IByteSource FromByteObservable(
        IObservable<byte[]> byteObservable,
        Func<Task<Maybe<long>>>? getLength = null)
    {
        return new ByteSource(byteObservable, getLength);
    }

    /// <summary>
    /// Creates a ByteSource from a synchronous Stream factory.
    /// The getLength function can provide a length if known.
    /// </summary>
    /// <param name="streamFactory">A factory method that returns a Stream to read from.</param>
    /// <param name="getLength">A function that can asynchronously provide the total length if known.</param>
    /// <returns>An IByteSource.</returns>
    public static IByteSource FromStreamFactory(
        Func<Stream> streamFactory,
        Func<Task<Maybe<long>>>? getLength = null)
    {
        return new ByteSource(
            Observable.Using(streamFactory, stream => stream.ToObservable()),
            getLength
        );
    }

    /// <summary>
    /// Creates a ByteSource from a string using a specified encoding and buffer size.
    /// The getLength function is computed based on the number of bytes for that string.
    /// </summary>
    /// <param name="str">The source string.</param>
    /// <param name="encoding">The text encoding to use. Defaults to UTF-8 if null.</param>
    /// <param name="bufferSize">Size of each emitted chunk.</param>
    /// <returns>An IByteSource.</returns>
    public static IByteSource FromString(
        string str,
        Encoding? encoding,
        int bufferSize = 4096)
    {
        encoding ??= Encoding.UTF8;
        var byteCount = encoding.GetByteCount(str);

        return new ByteSource(
            str.ToByteStream(encoding, bufferSize),
            () => Task.FromResult<Maybe<long>>(byteCount)
        );
    }
    
    public static IByteSource FromString(
        string str)
    {
        return new ByteSource(str.ToByteStream(Encoding.UTF8));
    }

    /// <summary>
    /// Creates a ByteSource from an asynchronous Stream factory.
    /// The getLength function can provide a length if known.
    /// </summary>
    /// <param name="streamFactory">A factory method that returns a Task of a Stream to read from.</param>
    /// <param name="getLength">A function that can asynchronously provide the total length if known.</param>
    /// <returns>An IByteSource.</returns>
    public static IByteSource FromAsyncStreamFactory(
        Func<Task<Stream>> streamFactory,
        Func<Task<Maybe<long>>>? getLength = null)
    {
        return new ByteSource(
            ObservableFactory.UsingAsync(streamFactory, stream => stream.ToObservable()),
            getLength
        );
    }

    /// <summary>
    /// Subscribes to the underlying IObservable of byte arrays.
    /// </summary>
    /// <param name="observer">Observer that will receive the byte arrays.</param>
    /// <returns>A disposable subscription.</returns>
    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return Bytes.Subscribe(observer);
    }
}