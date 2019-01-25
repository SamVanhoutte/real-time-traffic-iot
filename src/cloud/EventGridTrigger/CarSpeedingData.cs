using System;
using Newtonsoft.Json;

namespace EventGridTrigger
{
    public class CarSpeedingData
    {
        [JsonProperty("trajectid")]
        public string TrajectId { get; set; }
        [JsonProperty("detectiontime")]
        public DateTime DetectionTime { get; set; }
        [JsonProperty("lane")]
        public int Lane { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("licenseplate")]
        public string LicensePlate { get; set; }
        [JsonProperty("make")]
        public string Make { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("speed")]
        public double Speed { get; set; }
        [JsonProperty("speedlimit")]
        public int SpeedLimit { get; set; }
    }
}