using System;
using System.Linq;
using ElasticEPi.Logging;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using Nest;

namespace ElasticEPi.Indexing {
    public class SiteContentIndexer {

        private ReIndexContext _reIndexContext;
        private readonly IContentLoader _contentLoader;
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public SiteContentIndexer() {
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            _languageBranchRepository = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();
        }

        public int? BulkSize { get; set; }

        private ReIndexContext Context {
            get {
                if (_reIndexContext == null) {
                    return new ReIndexContext {
                        ContentReferences = _contentLoader.GetDescendents(ContentReference.RootPage),
                        Languages = _languageBranchRepository.ListEnabled().Select(x => x.Culture)
                    };
                }
                return _reIndexContext;
            }
        }


        internal int Start(Action<string> statusUpdateAction) {

            Logger.WriteToLog($"Starting BulkIndexer! Languages:{string.Join(", ",Context.Languages.Select(x=> x.TwoLetterISOLanguageName))}");
            var itemsIndexed = 0;

            foreach (var cultureInfo in Context.Languages) {
                Logger.WriteToLog($"Starting to bulk index {cultureInfo.TwoLetterISOLanguageName}");
                ContentLanguage.Instance.SetCulture(cultureInfo.TwoLetterISOLanguageName);

                var numberOfContentToIndex = Context.ContentReferences.Count();
                var bulkSize = BulkSize.GetValueOrDefault(99);
                for (int index = 0; (index*bulkSize) < numberOfContentToIndex; ++index) {

                    var itemsToTake = Context.ContentReferences.Skip(index*bulkSize).Take(bulkSize);
                    var languageSelector = new LanguageSelector(cultureInfo.Name);
                    var contentToIndex = _contentLoader.GetItems(itemsToTake, languageSelector).ToList();
                    contentToIndex = contentToIndex.Where(x => !x.IsDeleted).ToList();
                    if (contentToIndex.Any()) {
                        var bulkResponse = ContentIndexer.Instance.Index(contentToIndex);
                        itemsIndexed += contentToIndex.Count();
                        if (bulkResponse != null)
                            statusUpdateAction(bulkResponse.Errors.ToString());
                    }

                }
            }

            return itemsIndexed;
        }
    }
}
