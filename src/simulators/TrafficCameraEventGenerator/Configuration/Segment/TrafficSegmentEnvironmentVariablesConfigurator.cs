using System;
using System.Collections.Generic;
using System.Text;
using TrafficCameraEventGenerator.Configuration.Segment;
using NLog;
using TrafficCameraEventGenerator.Configuration.Settings;

namespace TrafficCameraEventGenerator.Configuration
{
    public class TrafficSegmentConfigurator : ITrafficSegmentConfigurator
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TrafficSegmentConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }


        public TrafficSegmentConfiguration GetConfiguration()
        {
            return new TrafficSegmentConfiguration
            {
                NumberOfLanes = _configurationReader.GetConfigValue("SEGMENT_LANE_COUNT", false, 3),
                AverageCarsPerMinute = _configurationReader.GetConfigValue("SEGMENT_AVG_CARS_PER_MINUTE", false, 60),
                SpeedLimit = _configurationReader.GetConfigValue("SEGMENT_SPEED_LIMIT", false, 120),
                RushHours = GetRushHours(),
                CameraDistance = _configurationReader.GetConfigValue("SEGMENT_CAMERA_DISTANCE", false, 2000),
                SpeedingPercentage = _configurationReader.GetConfigValue("SEGMENT_SPEEDING_PERCENTAGE", false, 2),
                MinSpeed = _configurationReader.GetConfigValue("SEGMENT_MIN_SPEED", false, 10),
                MaxSpeed = _configurationReader.GetConfigValue("SEGMENT_MAX_SPEED", false, 180)
            };
        }

        private IEnumerable<TimePeriod> GetRushHours()
        {
            var rushHours = new List<TimePeriod>
            {
                new TimePeriod("07:00", "08:00", true),
                new TimePeriod("17:00", "18:00", true)
            };
            string rushHourConfiguration = _configurationReader.GetConfigValue<string>("SEGMENT_RUSH_HOURS", false, null);
            bool rushHourOnlyInWeekDays = _configurationReader.GetConfigValue("SEGMENT_RUSH_HOURS_ONLYWEEKDAYS", false, true);
            if (!string.IsNullOrEmpty(rushHourConfiguration) && rushHourConfiguration.Contains("-"))
            {
                try
                {
                    rushHours = new List<TimePeriod>();
                    //notation : 07:00-08:00,17:30-18:30
                    foreach (var rushHourPeriod in rushHourConfiguration.Split(','))
                    {
                        if (rushHourPeriod.Contains("-"))
                        {
                            rushHours.Add(new TimePeriod(rushHourPeriod.Split('-')[0], rushHourPeriod.Split('-')[1], rushHourOnlyInWeekDays));
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
            return rushHours;
        }
    }


}
