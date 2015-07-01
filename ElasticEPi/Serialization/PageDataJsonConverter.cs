using System;
using System.Linq;
using System.Reflection;
using ElasticEPi.Configuration;
using ElasticEPi.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticEPi.Serialization {
    [ElasticJsonConverter]
    public class PageDataJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var jo = new JObject {
                {Constants.InheritanceFieldName, JToken.FromObject(value.GetType().GetInheritancHierarchy())}
            };
            foreach (PropertyInfo prop in value.GetType().GetProperties()) {
                if (prop.CanRead && !prop.GetIndexParameters().Any()) {
                    var propValue = prop.GetValue(value);
                    if (propValue != null) {
                        jo.Add(prop.Name.ToCamelCase(),  JToken.FromObject(propValue, serializer));
                    }
                }
            }
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var jToken = JToken.Load(reader);
            var contentLinkId = jToken.SelectToken("contentLink").Value<Int32>();

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            IContent content;
            if(contentLoader.TryGet(new ContentReference(contentLinkId),out content)) {
                return content;
            }
            return null;
        }

        public override bool CanConvert(Type objectType) {
            return typeof (PageData).IsAssignableFrom(objectType);
        }
    }
}
