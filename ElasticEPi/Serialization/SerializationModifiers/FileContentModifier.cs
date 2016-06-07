using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticEPi.Configuration;
using ElasticEPi.Extensions;
using ElasticEPi.Indexing;
using EPiServer.Core;
using Newtonsoft.Json.Linq;

namespace ElasticEPi.Serialization.SerializationModifiers
{
    public class FileContentModifier : IContentSerializationModifier
    {
        public void OnSerialization(IContent content, JObject jObject) {
            var mediaData = content as MediaData;
            if (mediaData == null) return;
            jObject.Add(Constants.FileContentFieldName, mediaData.ToBase64Content());
            
        }
    }
}
