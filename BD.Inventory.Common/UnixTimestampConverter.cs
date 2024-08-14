using Newtonsoft.Json;
using System;

namespace BD.Inventory.Common
{
    public class UnixTimestampConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dateTime)
            {
                long unixTimestamp = (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                writer.WriteValue(unixTimestamp);
            }
            else
            {
                throw new JsonSerializationException("Expected date object value.");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value.ToString() != null)
            {
                long ticks = (long)reader.Value * TimeSpan.TicksPerMillisecond;
                return new DateTime(1970, 1, 1).AddTicks(ticks);
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}