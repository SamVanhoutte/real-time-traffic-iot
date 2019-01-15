using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using TrafficCameraEventGenerator.Configuration.Settings;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class TrafficSegmentConfigurator : ITrafficSegmentConfigurator
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TrafficSegmentConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }


        public Task<TrafficSegmentConfiguration> GetConfiguration()
        {
            return Task.FromResult(new TrafficSegmentConfiguration
            {
                SegmentId = _configurationReader.GetConfigValue<string>("SEGMENT_ID", true),
                NumberOfLanes = _configurationReader.GetConfigValue("SEGMENT_LANE_COUNT", false, 3),
                AverageCarsPerMinute = _configurationReader.GetConfigValue("SEGMENT_AVG_CARS_PER_MINUTE", false, 60),
                SpeedLimit = _configurationReader.GetConfigValue("SEGMENT_SPEED_LIMIT", false, 120),
                RushHours = GetRushHours(),
                CameraDistance = _configurationReader.GetConfigValue("SEGMENT_CAMERA_DISTANCE", false, 2000),
                SpeedingPercentage = _configurationReader.GetConfigValue("SEGMENT_SPEEDING_PERCENTAGE", false, 2),
                MinSpeed = _configurationReader.GetConfigValue("SEGMENT_MIN_SPEED", false, 10),
                MaxSpeed = _configurationReader.GetConfigValue("SEGMENT_MAX_SPEED", false, 180)
            });
        }

        private IEnumerable<TimePeriod> GetRushHours()
        {
            var rushHours = new List<TimePeriod>
            {
                new TimePeriod("07:00", "08:00", true),
                new TimePeriod("17:00", "18:00", true)
            };
            string rushHourConfiguration = _configurationReader.GetConfigValue<string>("SEGMENT_RUSH_HOURS", false, null);
            return TimePeriod.ParseList(rushHourConfiguration);
        }
    }


}
