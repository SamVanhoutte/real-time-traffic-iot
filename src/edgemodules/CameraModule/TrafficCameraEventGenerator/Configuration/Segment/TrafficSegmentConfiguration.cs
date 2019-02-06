using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class TrafficSegmentConfiguration
    {
        public string SegmentId { get; set; }
        public int NumberOfLanes { get; set; } = 3;
        public int SpeedLimit { get; set; } = 120;
        [JsonConverter(typeof(RushHourConverter))]
        public IEnumerable<TimePeriod> RushHours { get; set; }
        public int MaxSpeed { get; set; } = 180;
        public int MinSpeed { get; set; } = 20;
        public int AverageCarsPerMinute { get; set; } = 30;
        public int SpeedingPercentage { get; set; } = 2;
        public int CameraDistance { get; set; } = 2000; // 2000m




        public TrafficSegmentConfiguration Clone()
        {
            return new TrafficSegmentConfiguration
            {
                SegmentId = SegmentId,
                NumberOfLanes = NumberOfLanes,
                AverageCarsPerMinute = AverageCarsPerMinute,
                SpeedLimit = SpeedLimit,
                RushHours = RushHours,
                SpeedingPercentage = SpeedingPercentage,
                CameraDistance = CameraDistance,
                MinSpeed = MinSpeed,
                MaxSpeed = MaxSpeed
            };
        }
    }
}
