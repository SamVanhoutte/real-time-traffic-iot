using System.Collections.Generic;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TrafficSegmentConfiguration : ITrafficSegmentConfiguration
    {
        public int NumberOfLanes { get; set; } = 3;
        public int SpeedLimit { get; set; } = 120;
        public IEnumerable<TimePeriod> RushHours { get; set; }
        public int MaxSpeed { get; set; } = 180;
        public int MinSpeed { get; set; } = 20;
        public int AverageCarsPerMinute { get; set; }
        public int SpeedingPercentage { get; set; }
        public int CameraDistance { get; set; } = 2000; // 2000m

        public static ITrafficSegmentConfiguration Busy =>
            new TrafficSegmentConfiguration
            {
                RushHours = new[] { new TimePeriod("05:00", "21:00", false) },
                AverageCarsPerMinute = 200,
                SpeedingPercentage = 6
            };

        public static ITrafficSegmentConfiguration Calm => new TrafficSegmentConfiguration
        {
            RushHours = new[] { new TimePeriod("07:00", "08:00", true), new TimePeriod("17:00", "18:00", true) },
            AverageCarsPerMinute = 20,
            SpeedingPercentage = 12
        };
    }
}
