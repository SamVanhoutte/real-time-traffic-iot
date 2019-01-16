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
        public int AverageCarsPerMinute { get; set; }
        public int SpeedingPercentage { get; set; }
        public int CameraDistance { get; set; } = 2000; // 2000m



        public bool IsRushHour(DateTime timeToEvaluate)
        {
            return RushHours.Any(rushHour => rushHour.Includes(timeToEvaluate));
        }
    }

    public class RushHourConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return TimePeriod.ParseList(reader.Value.ToString());

        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}
