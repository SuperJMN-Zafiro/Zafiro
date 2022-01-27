using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NodaTime;
using Xunit;
using Zafiro.Core.Intervals;

namespace Zafiro.Core.Tests
{
    public class IntervalTests
    {
        [Theory]
        [InlineData(1, 4, 3, 5, true)]
        [InlineData(1, 4, 7, 9, false)]
        public void Overlaps(int startA, int endA, int startB, int endB, bool expected)
        {

            var a = new Interval<int>(startA, endA);
            var b = new Interval<int>(startB, endB);

            a.Overlaps(b)
                .Should()
                .Be(expected);
        }

        [Theory]
        [InlineData(0, 3, 2, 4, 2, 3)]
        [InlineData(-1, 34, 0, 4, 0, 4)]
        [InlineData(0, 3, 4, 4, 0, 0)]
        [InlineData(10, 20, 12, 15, 12, 15)]
        public void Intersection(int startA, int endA, int startB, int endB, int startC, int endC)
        {

            var a = new Interval<int>(startA, endA);
            var b = new Interval<int>(startB, endB);

            var i = a.Intersection(b);

            i
                .Should()
                .Be(new Interval<int>(startC, endC));
        }

        [Theory]
        [InlineData(1, 3, 1, 2, false)]
        [InlineData(1, 4, 7, 9, false)]
        [InlineData(1, 2, 1, 2, true)]
        [InlineData(1, 2, 0, 5, true)]
        [InlineData(1, 5, 2, 5, false)]
        public void IsFullyContainedIn(int startA, int endA, int startB, int endB, bool expected)
        {

            var a = new Interval<int>(startA, endA);
            var b = new Interval<int>(startB, endB);

            a.IsFullyContainedIn(b)
                .Should()
                .Be(expected);
        }

        [Theory]
        [InlineData(1, 4, 3, 5, false)]
        [InlineData(1, 4, 7, 9, false)]
        [InlineData(1, 2, 1, 2, true)]
        [InlineData(1, 2, 0, 5, false)]
        [InlineData(1, 10, 2, 5, true)]
        public void ContainsFully(int startA, int endA, int startB, int endB, bool expected)
        {

            var a = new Interval<int>(startA, endA);
            var b = new Interval<int>(startB, endB);

            a.ContainsFully(b)
                .Should()
                .Be(expected);
        }

        [Theory]
        [MemberData(nameof(SimplifyData))]
        public void Simplify(IEnumerable<Interval<int>> input, IEnumerable<Interval<int>> expected)
        {
            input.Simplify().Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DateIntersection()
        {
            var zero = new DateTime(2020, 1, 1, 0, 0, 0);
            var twelve = new DateTime(2020, 1, 1, 12, 0, 0);
            
            var six = new DateTime(2020, 1, 1, 6, 0, 0);
            var eighteen = new DateTime(2020, 1, 1, 18, 0, 0);

            var a = new Interval<DateTime>(zero, twelve);
            var b = new Interval<DateTime>(six, eighteen);

            a.Intersection(b).Should()
                .Be(new Interval<DateTime>(six, twelve));
        }

        [Fact]
        public void IntervalIntersection()
        {
            var zero = Instant.FromDateTimeOffset(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var twelve = Instant.FromDateTimeOffset(new DateTimeOffset(2020, 1, 1, 12, 0,0, TimeSpan.Zero));
            var twentyfour = Instant.FromDateTimeOffset(new DateTimeOffset(2020, 1, 2, 0, 0, 0, TimeSpan.Zero));

            var i1 = new Interval(twelve, twentyfour);
            var i2 = new Interval(zero, twentyfour);

            var intersection = i1.Convert().Intersection(i2.Convert());
            intersection.Convert().Should().Be(new Interval(twelve, twentyfour));
        }

        [Theory]
        [InlineData(1, 4, 4, 7, false)]
        [InlineData(1, 4, 3.999, 7, true)]
        [InlineData(1, 4.0000001, 4, 7, true)]
        public void IntersectionSimple(double startA, double endA, double startB, double endB, bool expected)
        {
            var a = new Interval<double>(startA, endA);
            var b = new Interval<double>(startB, endB);

            var isIntersecting = !Equals(a.Intersection(b), Interval<double>.Empty);

            isIntersecting.Should().Be(expected);
        }

        public static IEnumerable<object[]> SimplifyData()
        {
            yield return new object[]
            {
                new[]{ new Interval<int>(4,8), new Interval<int>(5,9), new Interval<int>(1,2)},
                new[] { new Interval<int>(1,2), new Interval<int>(4,9) },
            };

            yield return new object[]
            {
                new[]{ new Interval<int>(4,6)},
                new[]{ new Interval<int>(4,6) },
            };

            yield return new object[]
            {
                new[]{ new Interval<int>(1,5), new Interval<int>(2,5)},
                new[] { new Interval<int>(1,5)},
            };
        }
    }
}