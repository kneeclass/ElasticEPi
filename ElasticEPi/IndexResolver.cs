using System;
using System.Globalization;
using ElasticEPi.Configuration;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace ElasticEPi {
    public abstract class IndexResolver {
        public static string GetIndex() {
            return ServiceLocator.Current.GetInstance<IndexResolver>().ResolveIndex();
        }
        internal abstract string ResolveIndex();
    }

    internal class ConfigurationIndexResolver : IndexResolver {
        internal override string ResolveIndex() {
            return ClientConfigurationSection.GetConfiguration().DefaultIndex;
        }
    }

}
