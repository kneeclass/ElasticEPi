using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Nest;

namespace ElasticEPi.Serialization.PreSerializationProccessing
{
    internal class RefactorFilter : IBuiltInFilter
    {
        public void ApplyFilter(ISearchRequest searchRequest) {

            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var infer = client.Infer;

            //no filter created by code
            if (searchRequest.Filter == null)
            {
                searchRequest.Filter = new FilterContainer();
                searchRequest.Filter.Bool = new BoolFilterDescriptor<IContent>();
                searchRequest.Filter.Bool.Must = new List<IFilterContainer>();
            }
            //filter created by code
            else if (searchRequest.Filter != null && searchRequest.Filter.Bool == null)
            {
                searchRequest.Filter.Bool = new BoolFilterDescriptor<IContent>();
                var mustList = new List<IFilterContainer>();
                if (searchRequest.Filter.Term != null)
                {
                    //keep termfilter created by code
                    var name = infer.PropertyPath(searchRequest.Filter.Term.Field);
                    mustList.Add(new FilterDescriptor<IContent>().Term(name, searchRequest.Filter.Term.Value));
                    searchRequest.Filter.Term = null;
                }
                //kepp termsfilter created by code
                if (searchRequest.Filter.Terms != null)
                {
                    var name = infer.PropertyPath(searchRequest.Filter.Terms.Field);
                    var terms = ((ITermsFilter)searchRequest.Filter.Terms).Terms;
                    mustList.Add(new FilterDescriptor<IContent>().Terms(name, terms.Cast<string>(), searchRequest.Filter.Terms.Execution));
                    searchRequest.Filter.Terms = null;
                }
                searchRequest.Filter.Bool.Must = mustList;
            }
        }
    }


}
