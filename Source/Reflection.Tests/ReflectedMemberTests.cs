using FluentAssertions;
using Xunit;

namespace Zafiro.Reflection.Tests
{
    public class ReflectedMemberTests
    {
        [Fact]
        public void Collection_is_detected()
        {
            var instance = new SomeModel();
            var sut = ReflectedMember.From(instance, model => model.Children);
            sut.IsCollection.Should().BeTrue();
        }

        [Fact]
        public void Non_collection_is_detected()
        {
            var instance = new SomeModel();
            var sut = ReflectedMember.From(instance, model => model.Children);
            sut.IsCollection.Should().BeTrue();
        }
    }
}