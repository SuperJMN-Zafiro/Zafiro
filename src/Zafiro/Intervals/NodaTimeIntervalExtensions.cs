using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Zafiro.Intervals
{
    public static class NodaTimeIntervalExtensions
    {
        public static Interval<Instant> Convert(this Interval self)
        {
            return new Interval<Instant>(self.HasStart ? self.Start : Instant.MinValue, self.HasEnd ? self.End : Instant.MaxValue);
        }

        public static Interval Convert(this Interval<Instant> self)
        {
            return new Interval(self.Start, self.End);
        }

        public static IEnumerable<Interval> Split(this Interval interval, Duration duration)
        {
            var steps = (int)(interval.Duration / duration);

            var intervals = Enumerable
                .Range(0, steps)
                .Select(i =>
                {
                    var intervalStart = interval.Start;
                    return new Interval(intervalStart + duration * i, intervalStart.Plus(duration * (i + 1)));
                });

            return intervals;
        }
    }
}