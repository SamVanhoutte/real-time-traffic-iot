using System;
using System.Collections.Generic;
using System.Linq;
using GuardNet;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class TimePeriod
    {
        public bool OnlyWeekdays { get; set; }
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;

        public TimePeriod(string startTime, string endTime, bool onlyWeekdays)
        {
            Guard.NotNullOrEmpty(startTime, nameof(startTime));
            Guard.NotNullOrEmpty(endTime, nameof(startTime));
            Guard.For<ArgumentException>(() => !startTime.Contains(":"), "StartTime should be of format hh:mm");
            Guard.For<ArgumentException>(() => !endTime.Contains(":"), "EndTime should be of format hh:mm");
            _startTime = new TimeSpan(int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 0);
            _endTime = new TimeSpan(int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 0);
            OnlyWeekdays = onlyWeekdays;
        }

        public bool Includes(DateTime timeToValidate)
        {
            // convert datetime to a TimeSpan
            TimeSpan timeOfDay = timeToValidate.TimeOfDay;
            // see if start comes before end
            if (_startTime < _endTime)
                return _startTime <= timeOfDay && timeOfDay <= _endTime;
            // start is after end, so do the inverse comparison
            return !(_endTime < timeOfDay && timeOfDay < _startTime);
        }

        public static IEnumerable<TimePeriod> ParseList(string timePeriodConfiguration)
        {
            var periods = new List<TimePeriod> { };
            if (timePeriodConfiguration == null) return periods;
            bool onlyOnWeekdays = true;
            foreach (var timePeriod in timePeriodConfiguration.Split(','))
            {
                if (timePeriod.Contains("-"))
                {
                    periods.Add(new TimePeriod(timePeriod.Split('-')[0], timePeriod.Split('-')[1], true));
                }
                else
                {
                    // check for boolean value to indicate weekdays
                    bool.TryParse(timePeriod, out onlyOnWeekdays);
                }
            }

            foreach (var timePeriod in periods)
            {
                timePeriod.OnlyWeekdays = onlyOnWeekdays;
            }

            return periods;
        }

        public TimeSpan TimeLeft(DateTime timeToValidate)
        {
            return _endTime.Subtract(timeToValidate.TimeOfDay);
        }
        public TimeSpan Duration => _endTime.Subtract(_startTime);
    }
}