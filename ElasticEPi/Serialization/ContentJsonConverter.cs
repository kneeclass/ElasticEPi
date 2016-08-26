using System;
using System.Linq;
using System.Reflection;
using ElasticEPi.Configuration;
using ElasticEPi.Extensions;
using ElasticEPi.Indexing;
using ElasticEPi.Logging;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticEPi.Serialization {
    
    // ReSharper disable once InconsistentNaming
    public class ContentJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var jo = new JObject();
            AddAdditionalFields(jo,value);
            foreach (PropertyInfo prop in value.GetType().GetProperties()) {
                if (prop.GetCustomAttribute(typeof(ElasticEPiIgnoreAttribute)) != null)
                    continue;
                if (prop.CanRead && !prop.GetIndexParameters().Any()) {
                    try {
                        var propValue = prop.GetValue(value);
                        if (propValue != null && jo.GetValue(prop.Name.ToCamelCase()) == null) {
                            jo.Add(prop.Name.ToCamelCase(), JToken.FromObject(propValue, serializer));
                        }
                    }
                    catch (Exception e) {
                        var icontent = value as IContent;
                        Logger.WriteToLog(
                            icontent != null
                                ? $"Failed to convert IContent to json. IContent information: Name={icontent.Name} Id={icontent.ContentLink.ID} Type={icontent.GetOriginalType().Name} "
                                : "Failed to convert IContent to json", Level.Error, e);
                    }
                }
            }
            jo.WriteTo(writer);
        }

        private void AddAdditionalFields(JObject jObject,object value) {
            var serializationModifiers = ContentIndexer.Instance.IndexingConventions.SerializationModifiers;
            foreach (var contentSerializationModifier in serializationModifiers) {
                try {
                    contentSerializationModifier.OnSerialization((IContent) value, jObject);
                }
                catch (Exception e) {
                    Logger.WriteToLog($"SerializationModifier failed: {contentSerializationModifier.GetType().Namespace}",Level.Error,e);
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var jToken = JToken.Load(reader);
            if (jToken.SelectToken("contentResolver") != null) {
                var contentResolverTypeAsStr = jToken.SelectToken("contentResolver").Value<string>();
                var type = Type.GetType(contentResolverTypeAsStr);
                if (type != null) {
                    var contentResolver = Activator.CreateInstance(type) as IElasticContentResolver;
                    return contentResolver?.CreateContent(jToken);
                }
            }
            var contentLinkId = jToken.SelectToken("contentLink").Value<int>();

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            IContent content;
            if(contentLoader.TryGet(new ContentReference(contentLinkId),out content)) {
                return content;
            }
            Logger.WriteToLog($"IContentJsonConverter failed to recreate IContent from contentLink. JSON: {jToken}",Level.Error);
            return null;
        }

        public override bool CanConvert(Type objectType) {
            return typeof (IContent).IsAssignableFrom(objectType);
        }
    }
}
