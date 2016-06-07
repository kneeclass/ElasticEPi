using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace ElasticEPi.Exceptions
{
    public class IndexException : Exception
    {
        public ElasticsearchServerError ServerError { get; set; }
        public IElasticsearchResponse RequestInformation { get; set; }
    }
}
