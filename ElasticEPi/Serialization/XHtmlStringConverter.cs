using System;
using ElasticEPi.Extensions;
using EPiServer.Core;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization {
    public class XHtmlStringConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var xhtmlstring = value as XhtmlString;
            if (xhtmlstring == null) return;
            writer.WriteValue(xhtmlstring.ToHtmlString().StripHtml());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof (XhtmlString);

        }
    }
}
