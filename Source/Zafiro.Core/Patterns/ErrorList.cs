using System.Collections.Generic;

namespace Zafiro.Core.Patterns
{
    public class ErrorList : List<string>
    {
        public ErrorList(IEnumerable<string> items) : base(items)
        {
        }

        public ErrorList(string item) : this(new[] { item })
        {

        }
    }
}