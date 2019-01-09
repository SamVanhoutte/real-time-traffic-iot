using System;
using GuardNet;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TimePeriod
    {
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;
        private readonly bool _onlyWeekdays;

        public TimePeriod(string startTime, string endTime, bool onlyWeekdays)
        {
            Guard.NotNullOrEmpty(startTime, nameof(startTime));
            Guard.NotNullOrEmpty(endTime, nameof(startTime));
            Guard.For<ArgumentException>(() => !startTime.Contains(":"), "StartTime should be of format hh:mm");
            Guard.For<ArgumentException>(() => !endTime.Contains(":"), "EndTime should be of format hh:mm");
            _startTime = new TimeSpan(int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 0);
            _endTime = new TimeSpan(int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 0);
            _onlyWeekdays = onlyWeekdays;
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
    }
}