using ElasticEPi.Configuration;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Extensions {
    public static class ElasticSearchClientExtensions {

        public static IIndexResponse IndexContent(this IElasticClient client, IContent content) {
            var configuration = ClientConfigurationSection.GetConfiguration();
            string languageIndex = null;
            if (content as ILocalizable != null) {
                 languageIndex = string.Format(Constants.LanguageIndexFormat, configuration.DefaultIndex, ((ILocalizable)content).Language.TwoLetterISOLanguageName);
            }

            return languageIndex != null
                ? client.Index(content, x => x.Id(content.ContentLink.ID).Index(languageIndex))
                : client.Index(content, y => y.Id(content.ContentLink.ID));
        }

    }
}
