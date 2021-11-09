using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Zafiro.Reflection.Tests
{
    public class ReflectedCollectionTests
    {
        [Fact]
        public void Non_collection_throws()
        {
            var instance = new SomeModel();
            var result = ReflectedCollection.From(instance, x => x.Int);
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Collection_does_not_throw()
        {
            var instance = new SomeModel();
            var result = ReflectedCollection.From(instance, x => x.Children);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Add_should_add()
        {
            var instance = new SomeModel();
            var sut = ReflectedCollection.From(instance, x => x.Children).Value;
            
            var child = new Child();
            sut.Add(child);
            sut.Items.Cast<object>().Should().Contain(child);
        }

        [Fact]
        public void Add_to_existing_collection_should_reuse_existing_collection()
        {
            var existingList = new List<Child>();
            var instance = new SomeModel() { Children = existingList };
            var sut = ReflectedCollection.From(instance, x => x.Children).Value;
            var child = new Child();
            sut.Add(child);
            sut.Items.Should().BeSameAs(existingList);
        }
    }
}