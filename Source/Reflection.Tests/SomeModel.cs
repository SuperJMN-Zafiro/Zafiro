using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Zafiro.Reflection.Tests
{
    class SomeModel
    {
        public string Text { get; set; }
        public int Int { get; set; }
        public string Combined { get; set; }
        public Maybe<int> OptionalInt { get; set; }
        public List<Child> Children { get; set; }
    }
}