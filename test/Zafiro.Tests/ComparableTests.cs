using FluentAssertions;
using Xunit;

namespace Zafiro.Tests
{
    public class ComparableTests
    {
        [Theory]
        [InlineData(2, 3, 3)]
        [InlineData(-2, 5, 5)]
        [InlineData(-2, -5, -2)]
        public void Max(int a, int b, int expected)
        {
            ComparableExtensions.Max(a, b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, 2)]
        [InlineData(-2, 5, -2)]
        [InlineData(-2, -5, -5)]
        public void Min(int a, int b, int expected)
        {
            ComparableExtensions.Min(a, b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, true)]
        [InlineData(-2, 5, true)]
        [InlineData(4, 4, false)]
        [InlineData(6, 4, false)]
        public void LessThan(int a, int b, bool expected)
        {
            ComparableExtensions.LessThan(a, b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, true)]
        [InlineData(-2, 5, true)]
        [InlineData(4, 4, true)]
        [InlineData(6, 4, false)]
        public void LessThanOrEqual(int a, int b, bool expected)
        {
            ComparableExtensions.LessThanOrEqual(a, b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, false)]
        [InlineData(-2, 5, false)]
        [InlineData(4, 4, false)]
        [InlineData(6, 4, true)]
        public void GreaterThan(int a, int b, bool expected)
        {
            ComparableExtensions.GreaterThan(a, b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, false)]
        [InlineData(-2, 5, false)]
        [InlineData(4, 4, true)]
        [InlineData(6, 4, true)]
        public void GreaterThanOrEqual(int a, int b, bool expected)
        {
            ComparableExtensions.GreaterThanOrEqual(a, b).Should().Be(expected);
        }
    }
}