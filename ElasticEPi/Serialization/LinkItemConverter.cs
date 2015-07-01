using System;
using EPiServer.SpecializedProperties;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization {
    class LinkItemConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            var linkItem = value as LinkItem;
            if (linkItem == null) return;

            serializer.Serialize(writer, new {
                linkItem.Href,
                linkItem.Text,
                linkItem.Title,
                linkItem.Target,
                linkItem.Language
            });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof (LinkItem);
        }
    }
}
