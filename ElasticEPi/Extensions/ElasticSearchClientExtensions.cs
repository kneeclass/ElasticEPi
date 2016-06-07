using ElasticEPi.Configuration;
using EPiServer.Core;
using Nest;

namespace ElasticEPi.Extensions {
    public static class ElasticSearchClientExtensions {

        public static IIndexResponse IndexContent(this IElasticClient client, IContent content) {
            string languageIndex = null;
            if (content as ILocalizable != null) {
                languageIndex = IndexResolver.GetIndex();
                    //(((ILocalizable) content).Language.TwoLetterISOLanguageName);
            }

            return languageIndex != null
                ? client.Index(content, x => x.Index(languageIndex))
                : client.Index(content);
        }

    }
}
