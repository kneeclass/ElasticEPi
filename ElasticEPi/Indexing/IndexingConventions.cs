using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ElasticEPi.Indexing
{
    public class IndexingConventions {

        public IndexingConventions() {
            SerializationModifiers = new List<IContentSerializationModifier>();
            JsonConverters = new List<JsonConverter>();
        }

        public List<IContentSerializationModifier> SerializationModifiers { get; set; }
        public List<JsonConverter> JsonConverters { get; set; }

    }
}
