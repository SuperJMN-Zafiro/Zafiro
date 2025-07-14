using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes;

public static class ByteSourceExtensions
{
    public static Stream ToStream(this IByteSource byteSource) => byteSource.Bytes.ToStream();
    public static Stream ToStreamSeekable(this IByteSource byteSource) => byteSource.Bytes.ToStreamSeekable();
    public static IObservable<Result> WriteTo(this IByteSource byteSource, Stream destination) => byteSource.Bytes.WriteTo(destination);
    public async static Task<Result> WriteTo(this IByteSource byteSource, string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return Result.Try(() =>
            {
                var directoryName = global::System.IO.Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            })
            .MapTry(() => File.WriteAllBytesAsync(path, byteSource.Array()));
    }
}