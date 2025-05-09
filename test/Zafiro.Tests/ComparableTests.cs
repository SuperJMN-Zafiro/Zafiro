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
            a.Max(b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, 2)]
        [InlineData(-2, 5, -2)]
        [InlineData(-2, -5, -5)]
        public void Min(int a, int b, int expected)
        {
            a.Min(b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, true)]
        [InlineData(-2, 5, true)]
        [InlineData(4, 4, false)]
        [InlineData(6, 4, false)]
        public void LessThan(int a, int b, bool expected)
        {
            a.LessThan(b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, true)]
        [InlineData(-2, 5, true)]
        [InlineData(4, 4, true)]
        [InlineData(6, 4, false)]
        public void LessThanOrEqual(int a, int b, bool expected)
        {
            a.LessThanOrEqual(b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, false)]
        [InlineData(-2, 5, false)]
        [InlineData(4, 4, false)]
        [InlineData(6, 4, true)]
        public void GreaterThan(int a, int b, bool expected)
        {
            a.GreaterThan(b).Should().Be(expected);
        }

        [Theory]
        [InlineData(2, 3, false)]
        [InlineData(-2, 5, false)]
        [InlineData(4, 4, true)]
        [InlineData(6, 4, true)]
        public void GreaterThanOrEqual(int a, int b, bool expected)
        {
            a.GreaterThanOrEqual(b).Should().Be(expected);
        }
    }
}