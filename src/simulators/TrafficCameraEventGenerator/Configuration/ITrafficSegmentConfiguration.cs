using System;
using System.Collections.Generic;
using System.Linq;
using Savanh.Extensions.Randoms;

namespace TrafficCameraEventGenerator.Configuration
{
    public interface ITrafficSegmentConfiguration
    {
        int NumberOfLanes { get; set; }
        int SpeedLimit { get; set; }
        IEnumerable<TimePeriod> RushHours { get; set; }
        int MaxSpeed { get; set; } 
        int MinSpeed { get; set; } 
        int AverageCarsPerMinute { get; set; }
        int SpeedingPercentage { get; set; }
        int CameraDistance { get; set; }

    }

    public static class TrafficSegmentConfigurationExtensions
    {
        public static bool IsRushHour(this ITrafficSegmentConfiguration segment, DateTime timeToEvaluate)
        {
            return segment.RushHours.Any(rushHour => rushHour.Includes(timeToEvaluate));
        }
    }
}
