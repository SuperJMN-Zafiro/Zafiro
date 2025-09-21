using System.Reactive.Linq;
using System.Security.Cryptography;
using Zafiro.Mixins;
using Zafiro.Reactive;
using Crc32 = System.IO.Hashing.Crc32;

namespace Zafiro.DivineBytes;

public static class ByteSourcePropertiesMixin
{
    /// <summary>
    /// Flattens to array
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Array(this IObservable<byte[]> data)
    {
        return data.ToEnumerable().Flatten().ToArray();
    }

    public static IObservable<long> GetSize(this IObservable<byte[]> data)
    {
        return data.Sum(bytes => (long)bytes.Length);
    }

    public static IObservable<uint> Crc32(this IObservable<byte[]> data)
    {
        return data.Aggregate(
                new Crc32(),
                (crc, chunk) =>
                {
                    crc.Append(chunk);
                    return crc;
                })
            .Select(crc => crc.GetCurrentHashAsUInt32());
    }

    public static IObservable<byte[]> Sha256(this IObservable<byte[]> data)
    {
        return data.Aggregate(
                new Sha256Accumulator(),
                (acc, chunk) => acc.Append(chunk)
            )
            .Select(acc => acc.GetHash());
    }

    public static IObservable<uint> Crc32(this IByteSource byteSource)
    {
        return byteSource.Bytes.Crc32();
    }

    public static IObservable<long> GetSize(this IByteSource byteSource)
    {
        return byteSource.Bytes.GetSize();
    }

    public static byte[] Array(this IByteSource byteSource)
    {
        return byteSource.Bytes.Array();
    }

    public static IObservable<byte[]> Sha256(this IByteSource byteSource)
    {
        return byteSource.Bytes.Sha256();
    }

    public class Sha256Accumulator
    {
        private readonly SHA256 sha256;
        private bool finished;

        public Sha256Accumulator()
        {
            sha256 = SHA256.Create();
            finished = false;
        }

        public Sha256Accumulator Append(byte[] chunk)
        {
            if (finished)
                throw new InvalidOperationException("No se pueden agregar datos una vez finalizado el hash.");

            sha256.TransformBlock(chunk, 0, chunk.Length, chunk, 0);
            return this;
        }

        public byte[] GetHash()
        {
            if (!finished)
            {
                // Finalizamos el procesamiento (es como decirle al SHA256: "¡Se acabó la función!")
                sha256.TransformFinalBlock([], 0, 0);
                finished = true;
            }
            return sha256.Hash ?? throw new InvalidOperationException();
        }
    }
}