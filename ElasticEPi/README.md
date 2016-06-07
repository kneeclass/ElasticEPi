#ElasticEPi

##Installation
Om du bara ska utveckla behöver du inte installera elasticsearch själv.
på Amanda finns en färdig elasticsearch installation
http://amanda:9200/

Men behöver ska du använda ElasticEPi på en skarp site eller något annat kan det vara bra att kunna sätta upp en egen.

###Installera Java :/
Installera JRE:n och kontrollera att JAVA_HOME system variablen pekar på din java installation

###Ladda hem Elasticsearch

Den version som ElasticEPi är utvecklat för är 1.7.4.
Det har kommit version 2.0 av Elasticsearch men då de paket som ElasticEPi använder sig utav inte riktigt hunnit med kör vi en ändre version för tillfället.
Nerladdnings länk: https://www.elastic.co/downloads/past-releases/elasticsearch-1-7-4
Extrahera din zip i lämplig mapp.

###Installera pluginen elasticsearch-mapper-attachments
Denna plugins används av elasticsearch för att kunna indexera och analysera filer.
Github: https://github.com/elastic/elasticsearch-mapper-attachments

Starta en commandopromt!
Navigera till bin mappen i Elasticsearch installationen ifrån förra steget.
Skriv följande kommando: plugin install elasticsearch/elasticsearch-mapper-attachments/2.7.1

###Start Elasticsearch!
Använd filen elasticsearch.bat i elasticsearchs bin mapp.
Det går även att installera elasticsearch som en tjänst på din maskin.

## Hur fungerar det?
ElasticEPi använder sig utav elastics officiella .NET versioner NEST och Elasticsearch.Net
När man som utvecklare ska ställa frågor med hjälpa av ElasticEPi är tanken att man ska använda sig utav NEST.
https://github.com/elastic/elasticsearch-net/tree/master/src/Nest

Det som ElasticEPi i huvudsak gör är att möjliggöra serilizering av IContent objekt in i elasticsearch.
För att hantera detta finns det både events uppsatta vid publicering av innehåll, samt ett schemalagtjobb för att indexera allt innehåll.

Utöver indexering finns det även en modul som skriver om de sökfrågor som ställs emot sökmotorn till att ta hänsyn till EPi specifika saker som så ACL:er, Type, publicerad osv..

##Hur gör man?
Börja med att lägga in den konfiguration som behövs
```xml
<configuration>
  <configSections>
    <section name="elasticEPi" type="ElasticEPi.Configuration.ClientConfigurationSection, ElasticEPi" />
  </configSections>
  <elasticEPi elasticSearchUrl="http://amanda:9200/" defaultIndex="dev-MySearchIndex" />
</configuration>
```

Sedan är det bara att söka:

```csharp
var client = ServiceLocator.Current.GetInstance<IElasticClient>();

var search = client.Search<BasePageData>(x => x
	.QueryString("*")
	.Filter(t =>t
		.Term(y => y.LanguageBranch, "sv")));
```

Den färdig prepade klienten finns att hämta ur ServiceLocatorn och innehåller allt som du behöver för att använda elasticsearch tillsammans med EPiServer.

Du behöver inte tänka på att skapa upp index i Elasticsearch, detta sköter paketet om själv. ElasticEPi kommer även sätta upp en mappning av de fält som den internt använder sig utav. i övrigt är det fritt fram att själv ändra mappning, analysers och allt annat godis man kan peta på.


### IContentSerializationModifier

Genom att implementera interfacet ```csharp IContentSerializationModifier ``` kan man påverka hur ditt IContent objekt serilizeras utan att behöva ändra i själva IContent objektet

Exempel:
```csharp
    public class EventOccasionStopDateModifier : IContentSerializationModifier {
        public void OnSerialization(IContent content, JObject jObject) {

            var eventOccasion = content as EventOccasionPage;
            if (eventOccasion == null) return;
            if (eventOccasion.EventStopDate.HasValue) return;
            jObject.Add(TypeExtensions.PropertyNameFor<EventOccasionPage>(x => x.
                EventStopDate).ToCamelCase(), 
                new DateTime(eventOccasion.EventStartDate.Year,eventOccasion.EventStartDate.Month,eventOccasion.EventStartDate.Day).AddDays(1));
        }
    }
``` 

Genom att att använda samma namn som egenskapen på IContent objektet kan man skriva över egenskaperna innan som serilizeras ner i elasticsearch.
Man kan även skapa helt nya egenskaper som inte finns i EPiServer och bara kommer finnas i elasticsearch.

## Custom JsonConverters

Registrera dina egna JsonConverters genom EPiServers IConfigurableModule
```csharp
[InitializableModule]
public class MyCustomJsonConverterInitialization : IConfigurableModule {
    public void Initialize(InitializationEngine context) {

    }
    public void Uninitialize(InitializationEngine context) {
        
    }
    public void ConfigureContainer(ServiceConfigurationContext context) {
        ContentIndexer.Instance.IndexingConventions.JsonConverters.Add(new MyCustomJsonConverter());
    }
}
``` 

### Misc

1. Använd [ElasticEPiIgnore] attributet för att ignorera en propertie

2. Block som ligger på sidor indexeras tillsammans med sidan. Dock endast sträng properties

3. trolololololol



### Mappning

Genom att använda chrome pluginen Sense är det lätt att skriva commandon emot din ElasticSearch server

Skapa ditt index:
```json
PUT myIndex
```

Kolla på din mappning, bör vara tom:
```json
GET myIndex/_mappings/
```

Skapa din mappning:
Detta är ett exempel på hur en mappning skulle kunna se ut.
$types, aCL, och filecontent är inbyggda typer i ElasticEPi och mappningen är lite känsligare. aCL och $types måste ha index = not_analyzed för att kunna göra korrekta filtreringar.
```json
PUT myIndex/_mapping/epicontent
{
    "properties" : {
		"$types" : {
			"type" : "string",
			"index" : "not_analyzed"
		},
		"aCL" : {
			"type" : "string",
			"index" : "not_analyzed"
		},
		"filecontent" : {
			"type" : "attachment",
            "analyzer" : "swedish",
            "fields" : {
                "filecontent"  : { "store" : "yes", "analyzer" : "swedish"},
                "author"   : { "store" : "yes" },
                "title"    : { "store" : "yes", "analyzer" : "swedish"},
                "date"     : { "store" : "yes" },
                "keywords" : { "store" : "yes", "analyzer" : "keyword" },
                "_name"    : { "store" : "yes" },
                "_content_type" : { "store" : "yes" }
          }
		},
		"mainBody" : {
			"type" : "string",
			"analyzer" : "swedish",
            "store": "yes"
		},
    	"heading" : {
			"type" : "string",
			"analyzer" : "swedish",
            "store": "yes"
		},
        "introduction" : {
			"type" : "string",
			"analyzer" : "swedish",
            "store": "yes"
		}
	}
}
```
