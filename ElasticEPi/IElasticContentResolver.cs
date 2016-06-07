using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using Newtonsoft.Json.Linq;

namespace ElasticEPi {
    public interface IElasticContentResolver {

        IContent CreateContent(JToken obj);

    }
}
