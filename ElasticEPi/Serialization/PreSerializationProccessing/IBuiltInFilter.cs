using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ElasticEPi.Serialization.PreSerializationProccessing
{
    interface IBuiltInFilter {
        void ApplyFilter(ISearchRequest searchRequest);
    }
}
