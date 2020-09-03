using System;
using System.Globalization;
using FluentAssertions.Common;
using NodaTime;

namespace Zafiro.Core.Tests
{
    public class DataBuilder
    {
        private readonly bool fixEnds;
        private readonly CultureInfo cultureInfo;

        public DataBuilder(bool fixEnds, CultureInfo cultureInfo = null)
        {
            this.fixEnds = fixEnds;
            this.cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        public Interval Interval(string startStr, string endStr)
        {
            var start = Instant(ParseExact(startStr));
            var end = Instant(ParseExact(endStr).AddDays(fixEnds ? 1 : 0));
            return new Interval(start, end);
        }

        private DateTime ParseExact(string start)
        {
            return DateTime.Parse(start, cultureInfo ?? CultureInfo.InvariantCulture);
        }

        public static Interval Interval(DateTime start, DateTime end)
        {
            return new Interval(Instant(start), Instant(end));
        }

        public static Interval AlwaysInterval => new Interval(null, null);

        public static Instant Instant(DateTime dateTime)
        {
            var dto = dateTime.ToDateTimeOffset(TimeSpan.Zero);
            return NodaTime.Instant.FromDateTimeOffset(dto);
        }
    }
}