using System.Configuration;

namespace ElasticEPi.Configuration {
    public class ClientConfigurationSection : ConfigurationSection {

        public static ClientConfigurationSection GetConfiguration() {
            var configuration = ConfigurationManager.GetSection("elasticEPi") as ClientConfigurationSection;
            if (configuration != null)
                return configuration;
            return new ClientConfigurationSection();
        }

        [ConfigurationProperty("elasticSearchUrl", IsRequired = true)]
        public string ElasticSearchUrl {
            get { return (string) base["elasticSearchUrl"]; }
        }

        [ConfigurationProperty("defaultIndex", IsRequired = true)]
        public string DefaultIndex {
            get { return (string)base["defaultIndex"]; }
        }

    }
}
