using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization
{
    public class ContentAreaJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var contentArea = (ContentArea) value;
            var items = new List<string>();
            foreach (var contentItem in contentArea.Items) {
                if(contentItem.ContentLink == ContentReference.EmptyReference) continue;
                if(contentItem.GetContent() == null) continue;
                items.Add(BlocksConverter.Convert(contentItem.GetContent()));
            }
            writer.WriteValue(string.Join(" ",items));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {

            return objectType == typeof (ContentArea);

        }
    }
}
