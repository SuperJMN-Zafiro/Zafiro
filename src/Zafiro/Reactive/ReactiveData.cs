using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Zafiro.Reactive
{
    public static class ReactiveData
    {
        private const int DefaultBufferSize = 4096;

        /// <summary>
        /// Creates an observable sequence from a stream factory.
        /// The stream's lifecycle is managed by the observable using Observable.Using.
        /// When the sequence completes or is disposed, the stream is automatically closed.
        /// </summary>
        /// <param name="streamFactory">A factory function that creates a Stream instance.</param>
        /// <param name="bufferSize">The size of the buffer used to read from the stream.</param>
        /// <returns>An observable sequence of byte arrays read from the stream.</returns>
        public static IObservable<byte[]> ToObservable(Func<Stream> streamFactory, int bufferSize = DefaultBufferSize)
        {
            return Observable.Using(
                streamFactory,
                stream => stream.ToObservable(bufferSize)
            );
        }

        /// <summary>
        /// Creates an observable sequence that reads from the given stream.
        /// Note: This extension method does NOT manage the stream's lifecycle.
        /// It is the caller's responsibility to close or dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="bufferSize">The size of the buffer used to read from the stream.</param>
        /// <returns>An observable sequence of byte arrays read from the stream.</returns>
        public static IObservable<byte[]> ToObservable(this Stream stream, int bufferSize = DefaultBufferSize)
        {
            return Observable.Defer(() =>
            {
                var buffer = new byte[bufferSize];

                return ReadRecursively();

                IObservable<byte[]> ReadRecursively() =>
                    Observable.FromAsync(() => stream.ReadAsync(buffer, 0, bufferSize))
                        .SelectMany(n => n == 0
                            ? Observable.Empty<byte[]>()
                            : Observable.Return(buffer.Take(n).ToArray())
                                .Concat(ReadRecursively()));
            });
        }
    }
}
