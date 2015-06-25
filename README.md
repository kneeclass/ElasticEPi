# ElasticEPi
EPiServer Elasticsearch client using [NEST](https://github.com/elastic/elasticsearch-net/tree/master/src/Nest)

##Installation

Nuget probably

##Configuration
Define a config section
```xml
<configSections>
  <section name="elasticEPi" type="ElasticEPi.Configuration.ClientConfigurationSection, ElasticEPi" />
<configSections/>
<elasticEPi elasticSearchUrl="http://www.your_elasticsearch_server.se:9200/" defaultIndex="epiindex" />
```

##Indexing

Standard NEST

##Settings

bla bla bla

##Querying

Standard NEST
