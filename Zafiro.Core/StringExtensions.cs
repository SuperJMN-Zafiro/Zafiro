using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zafiro.Core
{
    public static class StringExtensions
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

        public static IEnumerable<string> Split(this string str, int chunkSize)
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
    }
}