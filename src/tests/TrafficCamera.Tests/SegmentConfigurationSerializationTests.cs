using System;
using System.Linq;
using Newtonsoft.Json;
using TrafficCameraEventGenerator.Configuration.Segment;
using Xunit;

namespace TrafficCamera.Tests
{
    public class SegmentConfigurationSerializationTests
    {
        [Fact]
        public void SegmentConfigurationShouldDeserializeFromString()
        {
            string jsonValue = @"{
                  ""segmentId"": ""9"",
                  ""numberOfLanes"": 3,
                  ""speedLimit"": 120,
                  ""rushHours"": ""07:00-08:00,17:00-18:00"",
                  ""maxSpeed"": 166,
                  ""minSpeed"": 2,
                  ""averageCarsPerMinute"": 20,
                  ""speedingPercentage"": 3,
                  ""cameraDistance"": 2000
                }";

            var config = JsonConvert.DeserializeObject<TrafficSegmentConfiguration>(jsonValue);
            Assert.NotNull(config);
            Assert.Equal(2, config.RushHours.Count());
            Assert.True(config.IsRushHour(new DateTime(2019, 1, 1, 7, 1, 1)));
            Assert.True(config.IsRushHour(new DateTime(2019, 1, 1, 17, 59, 1)));
            Assert.False(config.IsRushHour(new DateTime(2019, 1, 1, 8, 1, 1)));
        }
    }
}
