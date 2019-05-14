using System.Text.RegularExpressions;

namespace Zafiro.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public static class StringExtensions
    {
        const string AbsoluteFilenameRegex = @"(([a-z]|[A-Z]):(?=\\(?![\0-\37<>:""/\\|?*])|\/(?![\0-\37<>:""/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:""/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+))((\\|\/)[^\0-\37<>:""/\\|?*]+|(\\|\/)$)*()\.\S*";

        public static string AsString(this IEnumerable<char> array)
        {
            return new string(array.ToArray());
        }

        public static IEnumerable<string> Split(this string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static string ExtractFileName(string str)
        {
            var pattern = @"(([a-z]|[A-Z]):(?=\\(?![\0-\37<>:""/\\|?*])|\/(?![\0-\37<>:""/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:""/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:""/\\|?*]+))((\\|\/)[^\0-\37<>:""/\\|?*]+|(\\|\/)$)*()\.\S*";
            return Regex.Match(str, pattern).Groups[0].Value;
        }

        public static IEnumerable<string> ExtractFileNames(string str)
        {
            return Regex.Matches(str, AbsoluteFilenameRegex).Cast<Match>().Select(x => x.Groups[0].Value);
        }
    }
}