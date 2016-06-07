using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticEPi.Indexing;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace ElasticEPi.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(SearchClientInitialization))]
    public class EPiServerEventsInitializer : IConfigurableModule {

        private bool _eventsAttached = false;

        public void Initialize(InitializationEngine context) {
            if (_eventsAttached == false) {
                var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
                var contentSecurityRepository = ServiceLocator.Current.GetInstance<IContentSecurityRepository>();

                //acl changed
                contentSecurityRepository.ContentSecuritySaved += (sender, arg) => {
                    var content = contentLoader.Get<IContent>(arg.ContentLink);
                    if (content.IsDeleted)
                        return;
                    ContentIndexer.Instance.Index(content);
                    var children = contentLoader.GetDescendents(arg.ContentLink);
                    //reindex children
                    foreach (var child in children) {
                        var childContent = contentLoader.Get<IContent>(child);
                        if(childContent.IsDeleted)
                            continue;
                        ContentIndexer.Instance.Index(childContent);
                    }
                };
                //content is Published
                contentEvents.PublishedContent += (sender, args) => ContentIndexer.Instance.Index(args.Content);

                //content deleted
                contentEvents.DeletedContent += (sender, args) => ContentIndexer.Instance.Delete(args.Content);

                //content moved
                contentEvents.MovedContent += (sender, args) => {
                    if (args.TargetLink.Equals(ContentReference.WasteBasket)) {
                        ContentIndexer.Instance.Delete(args.Content);
                        
                        var children = contentLoader.GetDescendents(args.ContentLink);
                        foreach (var child in children) {
                            var childContent = contentLoader.Get<IContent>(child);
                            ContentIndexer.Instance.Delete(childContent);
                        }
                    }
                    else
                        ContentIndexer.Instance.Index(args.Content);
                };
            }
        }

        public void Uninitialize(InitializationEngine context) {}

        public void ConfigureContainer(ServiceConfigurationContext context) {}
    }
}
