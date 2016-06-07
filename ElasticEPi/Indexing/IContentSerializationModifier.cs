using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using Newtonsoft.Json.Linq;

namespace ElasticEPi.Indexing
{
    public interface IContentSerializationModifier {
        void OnSerialization(IContent content, JObject jObject);
    }
}
