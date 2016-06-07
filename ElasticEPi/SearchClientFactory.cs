using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ElasticEPi.Configuration;
using ElasticEPi.Indexing;
using ElasticEPi.Logging;
using ElasticEPi.Serialization;
using ElasticEPi.Serialization.PreSerializationProccessing;
using EPiServer.Core;
using EPiServer.Logging;
using Nest;
using Newtonsoft.Json;
using StructureMap;

namespace ElasticEPi {
    public class SearchClientFactory {

        public static IElasticClient CreateClient(IContainer container) {
            var configuration = ClientConfigurationSection.GetConfiguration();
            var connectionSettings = new ConnectionSettings(
                new Uri(configuration.ElasticSearchUrl)
            );
            
            ConfigureIdProperty(connectionSettings);
            connectionSettings.SetJsonSerializerSettingsModifier(x => {
                x.Converters = ContentIndexer.Instance.IndexingConventions.JsonConverters;
                x.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            connectionSettings.SetDefaultTypeNameInferrer(x => typeof(IContent).IsAssignableFrom(x) ? Constants.EPiServerContentTypeName : null);
            connectionSettings.SetDefaultIndex(container.GetInstance<IndexResolver>().ResolveIndex());
            connectionSettings.ExposeRawResponse();
            container.Configure(x => x.For<ConnectionSettings>().Use(connectionSettings));

            return new ElasticClient(connectionSettings, null, new PreSerializationProccessor(connectionSettings));

        }

        private static void ConfigureIdProperty(ConnectionSettings settings) {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = new List<Type>();

            foreach (var assembly in assemblies) {
                try {
                    types.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException exception) {
                    Logger.WriteToLog($"ConfigureIdProperty partialy failed. Assembly={assembly.FullName}",Level.Information,exception);
                }
            }

            types = types.Where(x => typeof(IContent).IsAssignableFrom(x) && !x.IsInterface).ToList();
            settings.MapIdPropertyFor<IContent>(x => x.ContentLink);
            var idProperties = ((IConnectionSettingsValues)settings).IdProperties;
                

            var name = idProperties[typeof(IContent)];

            foreach (var contentType in types)
            {
                if (!idProperties.ContainsKey(contentType))
                {
                    ((IConnectionSettingsValues)settings).IdProperties.Add(contentType, name);
                }
            }
        }




    }
}
