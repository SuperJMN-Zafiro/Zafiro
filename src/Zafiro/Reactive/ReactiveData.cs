using System;
using System.Buffers;
using System.IO;
using System.Reactive.Disposables;
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
        /// Converts a Stream into an observable sequence of byte arrays.
        /// Uses optimized techniques to reduce garbage collector pressure
        /// and improve overall performance.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <param name="bufferSize">The size of the buffer used to read from the stream (80KB by default).</param>
        /// <returns>An observable sequence of byte arrays read from the stream.</returns>
        public static IObservable<byte[]> ToObservable(this Stream stream, int bufferSize = 81920) // 80KB by default
        {
            return Observable.Create<byte[]>(async (observer, cancellationToken) =>
            {
                // Use ArrayPool to reduce GC pressure
                byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);

                try
                {
                    // Prefer ReadAsync with Memory<T> for better performance in .NET Core/.NET 5+
                    Memory<byte> memoryBuffer = rentedBuffer;
                    int bytesRead;

                    // Use ValueTask to avoid Task allocation when operation completes synchronously
                    while ((bytesRead = await stream.ReadAsync(memoryBuffer, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        // Create a copy of the read segment
                        byte[] chunk = new byte[bytesRead];
                        rentedBuffer.AsSpan(0, bytesRead).CopyTo(chunk);

                        observer.OnNext(chunk);
                    }

                    observer.OnCompleted();
                }
                catch (OperationCanceledException)
                {
                    // Simply complete the sequence if canceled
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                finally
                {
                    // Always return the buffer to the pool
                    ArrayPool<byte>.Shared.Return(rentedBuffer);
                }

                return Disposable.Create(() => { cancellationToken.ThrowIfCancellationRequested(); });
            });
        }

        /// <summary>
        /// Alternative version that converts a Stream into an observable sequence of Memory<byte>.
        /// Avoids unnecessary memory copies by directly emitting references to the buffers.
        /// IMPORTANT: Requires that the consumer processes the data before the next emission,
        /// as the buffers are alternated and reused.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <param name="bufferSize">The size of the buffer used to read from the stream (80KB by default).</param>
        /// <returns>An observable sequence of Memory<byte> directly referencing the read data.</returns>
        public static IObservable<Memory<byte>> ToObservableMemory(this Stream stream, int bufferSize = 81920)
        {
            return Observable.Create<Memory<byte>>(async (observer, cancellationToken) =>
            {
                // Use 2 buffers to allow rotation without data loss
                byte[] buffer1 = ArrayPool<byte>.Shared.Rent(bufferSize);
                byte[] buffer2 = ArrayPool<byte>.Shared.Rent(bufferSize);
                byte[] currentBuffer = buffer1;

                try
                {
                    int bytesRead;
                    bool useBuffer1 = true;

                    while ((bytesRead = await stream.ReadAsync(
                               currentBuffer, 0, currentBuffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        // Emit the memory directly without copying
                        observer.OnNext(new Memory<byte>(currentBuffer, 0, bytesRead));

                        // Alternate between the two buffers for the next read
                        useBuffer1 = !useBuffer1;
                        currentBuffer = useBuffer1 ? buffer1 : buffer2;
                    }

                    observer.OnCompleted();
                }
                catch (OperationCanceledException)
                {
                    // Simply complete the sequence if canceled
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                finally
                {
                    // Always return the buffers to the pool
                    ArrayPool<byte>.Shared.Return(buffer1);
                    ArrayPool<byte>.Shared.Return(buffer2);
                }

                return Disposable.Create(() => { });
            });
        }
    }
}