using ElasticEPi.Configuration;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Extensions {
    public static class QueryDescriptorExtensions {

        public static SearchDescriptor<T> ContentLanguage<T>(this SearchDescriptor<T> searchDescriptor, string languageId) where T : class, IContent {
            var configuration = ClientConfigurationSection.GetConfiguration();
            
            searchDescriptor.Index(string.Format(Constants.LanguageIndexFormat, configuration.DefaultIndex, languageId));
            return searchDescriptor;
        }

    }
}
