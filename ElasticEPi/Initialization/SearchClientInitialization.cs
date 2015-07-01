using System;
using ElasticEPi.Configuration;
using ElasticEPi.Serialization;
using ElasticEPi.Serialization.PreSearchModifiers;
using EPiServer.Core;
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

            var configuration = ClientConfigurationSection.GetConfiguration();

            var settings = new ConnectionSettings(
                new Uri(configuration.ElasticSearchUrl),
                configuration.DefaultIndex
                );
            settings.SetDefaultTypeNameInferrer(x => typeof(IContent).IsAssignableFrom(x) ? Constants.EPiServerContentTypeName : null);
            settings.SetJsonSerializerSettingsModifier(x => {
                x.Converters = new JsonConverter[] {
                    new PageDataJsonConverter(), 
                    new PropertyDataCollectionJsonConverter(),
                    new LinkItemConverter()
                };
                x.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            //if debug ?
            settings.ExposeRawResponse();
            var elasticClient = new ElasticClient(settings,null, new NestInheritenceSerializer(settings));
            context.Container.Configure(x => x.For<ConnectionSettings>().Use(settings));
            context.Container.Configure(x => x.For<IElasticClient>().Use(elasticClient));
        }


    }
}
