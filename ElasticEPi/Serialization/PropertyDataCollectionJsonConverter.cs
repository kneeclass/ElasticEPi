using System;
using EPiServer.Core;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization {
    class PropertyDataCollectionJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            //We do not want so serialize PropertyDataCollections
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof (PropertyDataCollection);
        }
    }
}
