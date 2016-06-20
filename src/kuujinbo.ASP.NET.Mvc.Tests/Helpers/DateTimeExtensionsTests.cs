using System;
using kuujinbo.ASP.NET.Mvc.Helpers;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Helpers
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void NextDayInclusive_StartAndWantedSame_ReturnsStart()
        {
            var start = DateTime.Today;

            Assert.Equal(start, start.NextDayInclusive(start.DayOfWeek));
        }

        [Fact]
        public void NextDayInclusive_StartAndWantedSame_IgnoresTimePartAndReturnsStart()
        {
            var start = DateTime.Now;
            var result = start.NextDayInclusive(start.DayOfWeek);

            Assert.Equal(
                new DateTime(start.Year, start.Month, start.Day), 
                result
            );
        }

        [Fact]
        public void NextDayInclusive_StartLessThanWanted_ReturnsNextWantedDayOfWeek()
        {
            var start = DateTime.Today;
            var offset = 4;
            var wanted = start.AddDays(offset).DayOfWeek;
            var result = start.NextDayInclusive(wanted);

            Assert.Equal(offset, (result - start).Days);
            Assert.Equal(wanted, result.DayOfWeek);
        }

        [Fact]
        public void NextDayInclusive_StartGreaterThanWanted_ReturnsNextWantedDayOfWeek()
        {
            var start = DateTime.Today;
            var offset = -1;
            var wanted = start.AddDays(offset).DayOfWeek;
            var result = start.NextDayInclusive(wanted);

            Assert.Equal(offset + 7, (result - start).Days);
            Assert.Equal(wanted, result.DayOfWeek);
        }
    }
}