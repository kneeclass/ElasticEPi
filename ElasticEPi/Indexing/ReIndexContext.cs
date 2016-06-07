using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;

namespace ElasticEPi.Indexing {
    public class ReIndexContext {
        public IEnumerable<ContentReference> ContentReferences { get; set; }
        public IEnumerable<CultureInfo> Languages { get; set; } 
    }
}
