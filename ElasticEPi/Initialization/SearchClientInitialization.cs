using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ElasticEPi.Configuration;
using ElasticEPi.Indexing;
using ElasticEPi.Serialization;
using ElasticEPi.Serialization.SerializationModifiers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Nest;
using Newtonsoft.Json;

namespace ElasticEPi.Initialization {
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class SearchClientInitialization : IConfigurableModule {

        public void Initialize(InitializationEngine context) {}
        public void Uninitialize(InitializationEngine context) {}
        public void Preload(string[] parameters) { }

        public void ConfigureContainer(ServiceConfigurationContext context) {

            var configurationIndexResolver = new ConfigurationIndexResolver();
            context.Container.Configure(x => x.For<IndexResolver>().Use(configurationIndexResolver));

            //Modifiers
            ContentIndexer.Instance.IndexingConventions.SerializationModifiers.Add(new TypesModifiers());
            ContentIndexer.Instance.IndexingConventions.SerializationModifiers.Add(new AclModifier());
            ContentIndexer.Instance.IndexingConventions.SerializationModifiers.Add(new FileContentModifier());

            //JsonConverters
            ContentIndexer.Instance.IndexingConventions.JsonConverters.AddRange(new JsonConverter[] {
                new ContentJsonConverter(),
                new ContentReferenceJsonConverter(),
                new PropertyDataCollectionJsonConverter(),
                new LinkItemConverter(),
                new TypeConverter(),
                new AccessControlListJsonConverter(),
                new XHtmlStringConverter(),
                new ContentAreaJsonConverter(),
                new ContentMixinJsonConverter()
            });


            context.ConfigurationComplete += AddElasticClientToContainer;

        }

        private void AddElasticClientToContainer(object sender, ServiceConfigurationEventArgs e) {
            var client = SearchClientFactory.CreateClient(e.Container);
            e.Container.Configure(x => x.For<IElasticClient>().Use(client));
        }
    }
}
