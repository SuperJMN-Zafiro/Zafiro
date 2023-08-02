using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zafiro.Mixins
{
    public static class StringMixin
    {
        private const string AbsoluteFilenameRegex =
            @"(([a-z]|[A-Z]):(?=\\(?![\0-\37<>:""/\\|?*])|\/(?![\0-\37<>:""/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:""/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+))((\\|\/)[^\0-\37<>:""/\\|?*]+|(\\|\/)$)*()\.\S*";

        public static string AsString(this IEnumerable<char> array)
        {
            return new string(array.ToArray());
        }

        public static string Join(this IEnumerable<string> strings, string separator = "\n")
        {
            return string.Join(separator, strings);
        }

        public static IEnumerable<string> Chunkify(this string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static string ExtractFileName(string str)
        {
            var pattern =
                @"(([a-z]|[A-Z]):(?=\\(?![\0-\37<>:""/\\|?*])|\/(?![\0-\37<>:""/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:""/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+))((\\|\/)[^\0-\37<>:""/\\|?*]+|(\\|\/)$)*()\.\S*";
            return Regex.Match(str, pattern).Groups[0].Value;
        }

        public static IEnumerable<string> ExtractFileNames(string str)
        {
            return Regex.Matches(str, AbsoluteFilenameRegex).Cast<Match>().Select(x => x.Groups[0].Value);
        }

        public static string FindDuplicateSubstring(string s, bool allowOverlap = false)
        {
            int matchPos = 0, maxLength = 0;
            for (var shift = 1; shift < s.Length; shift++)
            {
                var matchCount = 0;
                for (var i = 0; i < s.Length - shift; i++)
                {
                    if (s[i] == s[i + shift])
                    {
                        matchCount++;
                        if (matchCount > maxLength)
                        {
                            maxLength = matchCount;
                            matchPos = i - matchCount + 1;
                        }

                        if (!allowOverlap && matchCount == shift)
                        {
                            // we have found the largest allowable match 
                            // for this shift.
                            break;
                        }
                    }
                    else
                    {
                        matchCount = 0;
                    }
                }
            }

            if (maxLength > 0)
            {
                return s.Substring(matchPos, maxLength);
            }

            return null;
        }

        public static string Replace(this string input, int index, int length, string replacement)
        {
            var builder = new StringBuilder();
            builder.Append(input.Substring(0, index));
            builder.Append(replacement);
            builder.Append(input.Substring(index + length));
            return builder.ToString();
        }

        public static IEnumerable<string> Lines(this string s)
        {
            using var tr = new StringReader(s);
            while (tr.ReadLine() is { } l)
            {
                yield return l;
            }
        }

        public static string JoinWithLines<T>(this IEnumerable<T> items)
        {
            return Join(items, Environment.NewLine);
        }

        public static string JoinWithCommas<T>(this IEnumerable<T> items)
        {
            return Join(items, ", ");
        }

        public static string Join<T>(this IEnumerable<T> items, string separator)
        {
            return string.Join(separator, items.Select(x => x?.ToString()));
        }
    }
}