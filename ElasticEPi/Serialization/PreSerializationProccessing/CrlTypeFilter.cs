using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticEPi.Configuration;
using ElasticEPi.Extensions;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Serialization.PreSerializationProccessing
{
    internal class CrlTypeFilter : IBuiltInFilter
    {
        public void ApplyFilter(ISearchRequest searchRequest) {
            ((List<IFilterContainer>)searchRequest.Filter.Bool.Must).Add(
                new FilterDescriptor<IContent>().Term(Constants.InheritanceFieldName, searchRequest.ClrType.ToSimpleAssemblyName())
            );
        }
    }
}
