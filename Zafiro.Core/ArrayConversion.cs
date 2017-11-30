using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Core
{
    public static class ArrayConversion
    {
        public static byte[] ToByteArray(this int[] buffer)
        {
            var q = from i in buffer
                let bytes = GetBytes(i)
                from b in bytes
                select b;

            return q.ToArray();
        }
        public static int[] ToIntArray(this byte[] buffer)
        {
            var q = from b in buffer.Chunkify(sizeof(int))
                select BitConverter.ToInt32(b.ToArray(), 0);

            return q.ToArray();
        }


        private static IEnumerable<byte> GetBytes(int i)
        {
            return BitConverter.GetBytes(i);
        }
    }
}