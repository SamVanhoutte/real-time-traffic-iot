using System;
using System.Collections.Generic;
using System.Text;
using TrafficCameraEventGenerator.Configuration.Segment;
using Xunit;

namespace TrafficCamera.Tests
{
    public class TimePeriodParsingTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NullOrEmptyString_ShouldReturnEmptyList(string timePeriodString)
        {
            var timePeriodList = TimePeriod.ParseList(timePeriodString);
            Assert.NotNull(timePeriodList);
            Assert.Empty(timePeriodList);
        }

        [Theory]
        [InlineData("7:00-8:00,8:15-10:00")]
        [InlineData("7:00-8:00")]
        public void ValueWithoutBoolean_ShouldOnlyHaveWeekdayPeriods(string timePeriodString)
        {
            var timePeriodList = TimePeriod.ParseList(timePeriodString);
            foreach (var timePeriod in timePeriodList)
            {
                Assert.True(timePeriod.OnlyWeekdays);
            }
        }

        [Theory]
        [InlineData("7:00-8:00,true,8:15-10:00")]
        [InlineData("7:00-8:00,true")]
        [InlineData("true,7:00-8:00")]
        public void ValueWithTrueValue_ShouldOnlyHaveWeekdayPeriods(string timePeriodString)
        {
            var timePeriodList = TimePeriod.ParseList(timePeriodString);
            foreach (var timePeriod in timePeriodList)
            {
                Assert.True(timePeriod.OnlyWeekdays);
            }
        }

        [Theory]
        [InlineData("7:00-8:00,false,8:15-10:00")]
        [InlineData("7:00-8:00,false")]
        [InlineData("false,7:00-8:00")]
        public void ValueWithFalseValue_ShouldOnlyHaveWeekdayPeriods(string timePeriodString)
        {
            var timePeriodList = TimePeriod.ParseList(timePeriodString);
            foreach (var timePeriod in timePeriodList)
            {
                Assert.False(timePeriod.OnlyWeekdays);
            }
        }
    }
}
