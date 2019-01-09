using System.Collections.Generic;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TrafficSegmentConfiguration : ITrafficSegmentConfiguration
    {
        public int NumberOfLanes { get; set; }
        public int SpeedLimit { get; set; }
        public IEnumerable<TimePeriod> RushHours { get; set; }
        public int MaxSpeed { get; set; }
        public int MinSpeed { get; set; }
        public int AverageCarsPerMinute { get; set; }
        public int SpeedingPercentage { get; set; }
        public int CameraDistance { get; set; }
    }
}
