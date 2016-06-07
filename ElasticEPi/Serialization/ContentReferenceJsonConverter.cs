using System;
using EPiServer.Core;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization
{
    public class ContentReferenceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contentRef = value as ContentReference;
            if (contentRef == null) return;
            writer.WriteValue(contentRef.ID);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ContentReference).IsAssignableFrom(objectType);
        }
    }
}