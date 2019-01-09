using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class TrafficSegmentConfiguration
    {
        public string SegmentId { get; set; }
        public int NumberOfLanes { get; set; } = 3;
        public int SpeedLimit { get; set; } = 120;
        public IEnumerable<TimePeriod> RushHours { get; set; }
        public int MaxSpeed { get; set; } = 180;
        public int MinSpeed { get; set; } = 20;
        public int AverageCarsPerMinute { get; set; }
        public int SpeedingPercentage { get; set; }
        public int CameraDistance { get; set; } = 2000; // 2000m

        public static TrafficSegmentConfiguration Busy =>
            new TrafficSegmentConfiguration
            {
                SegmentId = Guid.NewGuid().ToString("N"),
                RushHours = new[] { new TimePeriod("05:00", "21:00", false) },
                AverageCarsPerMinute = 200,
                SpeedingPercentage = 6
            };

        public static TrafficSegmentConfiguration Calm => new TrafficSegmentConfiguration
        {
            SegmentId = Guid.NewGuid().ToString("N"),
            RushHours = new[] { new TimePeriod("07:00", "08:00", true), new TimePeriod("17:00", "18:00", true) },
            AverageCarsPerMinute = 20,
            SpeedingPercentage = 12
        };

        public bool IsRushHour(DateTime timeToEvaluate)
        {
            return RushHours.Any(rushHour => rushHour.Includes(timeToEvaluate));
        }
    }
}
