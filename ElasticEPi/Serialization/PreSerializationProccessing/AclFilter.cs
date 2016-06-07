using System.Collections.Generic;
using System.Linq;
using ElasticEPi.Configuration;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Serialization.PreSerializationProccessing
{
    internal class AclFilter : IBuiltInFilter
    {
        public void ApplyFilter(ISearchRequest searchRequest) {
            var principalInfo = EPiServer.Security.PrincipalInfo.Current;
            var mustlist = ((List<IFilterContainer>) searchRequest.Filter.Bool.Must);
            if (principalInfo == null || principalInfo.RoleList.All(string.IsNullOrEmpty))
                mustlist.Add(new FilterDescriptor<IContent>().Term(Constants.AclFieldName, "Everyone"));
            else {
                var list = new List<string>(principalInfo.RoleList) { "Everyone" };
                mustlist.Add(new FilterDescriptor<IContent>().Terms(Constants.AclFieldName, list,
                    TermsExecution.Or));
            }
        }
    }
}
