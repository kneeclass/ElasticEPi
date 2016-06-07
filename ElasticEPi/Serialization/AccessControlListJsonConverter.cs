using System;
using System.Linq;
using EPiServer.Security;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization {
    public class AccessControlListJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            /* We serialize ACLS in IContentJsonConverter if the content implements IContentSecurable */

            //var acl = value as AccessControlList;
            //if (acl == null) return;
            //serializer.Serialize(writer, acl.Entries.Where(x=> (x.Access & AccessLevel.Read) != 0).Select(x=> x.Name));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {

            return typeof (AccessControlList).IsAssignableFrom(objectType);
        }
    }
}
