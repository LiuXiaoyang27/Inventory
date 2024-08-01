using Newtonsoft.Json;
using System;

namespace BD.Inventory.Common
{
    public class UnixTimestampConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
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