using System.Linq;
using ElasticEPi.Configuration;
using ElasticEPi.Indexing;
using EPiServer.Core;
using EPiServer.Security;
using Newtonsoft.Json.Linq;

namespace ElasticEPi.Serialization.SerializationModifiers
{
    public class AclModifier : IContentSerializationModifier
    {
        public void OnSerialization(IContent content, JObject jObject) {
            var securable = content as IContentSecurable;
            if (securable == null) return;
            var entries = securable.GetContentSecurityDescriptor().Entries;
            var acls = entries.Where(x => (x.Access & AccessLevel.Read) != 0).Select(x => x.Name);
            jObject.Add(Constants.AclFieldName, new JArray(acls));
            
        }
    }
}
