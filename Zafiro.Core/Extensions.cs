namespace Zafiro.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class Extensions
    {
        public static Stream FromUtf8ToStream(this string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        public static string ToString(this IEnumerable items)
        {
            var builder = new StringBuilder();

            foreach (var item in items)
            {
                builder.Append(" ·" + item + "\n");
            }

            return builder.ToString();
        }

        public static string AsNumberedList<T>(this IEnumerable<T> enumerable)
        {
            var partitionsList = string.Join("\n", enumerable.Select((item, i) => $"{i + 1} - {item}"));
            return partitionsList;
        }

        public static void AddAll<T>(this IAdd<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static Tuple<string, string> Dicotomize(this string str, char ch)
        {
            var indexOfChar = str.IndexOf(ch);

            if (indexOfChar < 0)
            {
                return new Tuple<string, string>(str, null);
            }

            var leftPart = str.Substring(0, indexOfChar);
            indexOfChar++;
            var rightPart = str.Substring(indexOfChar, str.Length - indexOfChar);

            return new Tuple<string, string>(leftPart, rightPart);
        }

        public static Stream ToStream(this string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }


        public static IEnumerable<TResult> GatherAttributes<TAttribute, TResult>(this IEnumerable<Type> types, Func<Type, TAttribute, TResult> converter)
            where TAttribute : Attribute
        {
            return from type in types
                let att = type.GetTypeInfo().GetCustomAttribute<TAttribute>()
                where att != null
                select converter(type, att);
        }

        public static IEnumerable<TResult> GatherAttributesFromMembers<TAttribute, TResult>(this IEnumerable<Type> types,
            Func<PropertyInfo, TAttribute, TResult> converter)
            where TAttribute : Attribute
        {
            return from type in types
                from member in type.GetRuntimeProperties()
                let att = member.GetCustomAttribute<TAttribute>()
                where att != null
                select converter(member, att);
        }

        public static IEnumerable<IEnumerable<T>> Chunkify<T>(this IEnumerable<T> source,
            int chunkSize)
        {
            IEnumerator<T> e = source.GetEnumerator();
            Func<bool> mover = () => e.MoveNext();
            int count = 0;
            while (mover())
            {
                List<T> chunk = new List<T>(chunkSize);
                do
                {
                    chunk.Add(e.Current);
                } while (++count < chunkSize && e.MoveNext());
                yield return chunk;
                count = 0;
            }
        }
    }
}