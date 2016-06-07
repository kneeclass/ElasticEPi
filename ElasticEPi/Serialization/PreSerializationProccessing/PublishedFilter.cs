using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Serialization.PreSerializationProccessing
{
    public class PublishedFilter : IBuiltInFilter
    {
        public void ApplyFilter(ISearchRequest searchRequest) {
            if (searchRequest.Filter.Bool.MustNot == null) {
                searchRequest.Filter.Bool.MustNot = new List<IFilterContainer>();
            }
            ((List<IFilterContainer>)searchRequest.Filter.Bool.MustNot).Add(
                new FilterDescriptor<IVersionable>().Range(x => x.OnField(o => o.StartPublish).GreaterOrEquals(DateTime.Now.ToString("s")))
            );

            ((List<IFilterContainer>)searchRequest.Filter.Bool.MustNot).Add(
                new FilterDescriptor<IVersionable>().Range(x => x.OnField(o => o.StopPublish).LowerOrEquals(DateTime.Now.ToString("s")))
            );
        }
    }
}
