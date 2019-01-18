using System;
using Newtonsoft.Json;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
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