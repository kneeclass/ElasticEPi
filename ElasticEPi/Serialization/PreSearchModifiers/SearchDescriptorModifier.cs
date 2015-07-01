using System;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Serialization.PreSearchModifiers {
    public class SearchDescriptorModifier : IPreSearchModifier {

        public void ModifySearch(object data) {

            var searchRequest = (ISearchRequest) data;

            if (searchRequest.Filter == null) {
                searchRequest.Filter = new FilterContainer(
                    new TermFilter {
                        Field = new PropertyPathMarker {
                            Name = "languageBranch"
                        },
                        Value = "sv"
                    });
            }
            else {
                //((IFilterContainer) searchRequest.Filter);
            }

            var dothings = "asd";

        }

        public Type ModifysType {
            get { return typeof (SearchDescriptor<IContent>); }
        }
    }
}
