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

       

        public bool IsRushHour(DateTime timeToEvaluate)
        {
            return RushHours.Any(rushHour => rushHour.Includes(timeToEvaluate));
        }
    }
}
