using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Zafiro.Reflection.Tests
{
    public class CollectionTypeTests
    {
        [Fact]
        public void Collection_fails()
        {
            var result = CollectionType.Create(typeof(ICollection<string>));
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Add_succeeds()
        {
            var strings = new List<string>();
            var result = CollectionType.Create(strings.GetType());
            result.Value.Add(strings, "hola");

            strings.Should().Contain("hola");
        }

        [Fact]
        public void Adding_int_to_collection_of_strings_fails()
        {
            var strings = new List<string>();
            var ct = CollectionType.Create(strings.GetType()).Value;
            var result = ct.Add(strings, 2);
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Non_collection_fails()
        {
            var result = CollectionType.Create(typeof(string));
            result.IsFailure.Should().BeTrue();
        }
    }
}