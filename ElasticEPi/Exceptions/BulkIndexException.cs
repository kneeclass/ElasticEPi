using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ElasticEPi.Exceptions
{
    public class BulkIndexException : Exception {
        public IBulkResponse BulkResponse { get; set; }
    }
}
