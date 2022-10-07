using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Zafiro.XPath.Tests
{
    class SomeModel
    {
        public string Text { get; set; }
        public int Int { get; set; }
        public string Combined { get; set; }
        public Maybe<int> OptionalInt { get; set; }
        public List<Child> Children { get; set; }
        public List<string> Strings { get; set; }
        public List<int> Ints { get; set; }
    }
}