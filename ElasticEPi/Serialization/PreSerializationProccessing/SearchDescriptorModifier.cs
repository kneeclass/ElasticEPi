using System;
using System.Collections.Generic;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Serialization.PreSerializationProccessing {
    public class SearchDescriptorModifier : IPreSearchModifier {

        private static readonly List<IBuiltInFilter> BuildInFilters = new List<IBuiltInFilter> {
            new RefactorFilter(), //must be first
            new CrlTypeFilter(),
            new AclFilter(),
            new PublishedFilter()
        }; 

        public void ModifySearch(object data) {
            var searchRequest = (ISearchRequest) data;

            if (!typeof (IContent).IsAssignableFrom(searchRequest.ClrType)) return;


            foreach (var buildInFilter in BuildInFilters) {
                buildInFilter.ApplyFilter(searchRequest);
            }

        }

        public Type ModifysType => typeof (SearchDescriptor<IContent>);
    }
}
