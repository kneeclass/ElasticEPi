using System;
using System.Linq;
using ElasticEPi.Indexing;
using ElasticEPi.Initialization;
using EPiServer.ServiceLocation;
using Nest;
using Xunit;
using ElasticEPi.Test.EPiServerInitialization;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Newtonsoft.Json;

namespace ElasticEPi.Test {
    public class MiscTests : IClassFixture<EPiServerInitializer> {
        
        public MiscTests(EPiServerInitializer initializer) {

            var applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            initializer.Initialize(applicationPath);

        }

        [Fact]
        public void Client_Not_Null() {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            Assert.NotNull(client);
        }


        [Fact]
        public void Returns_SearchResult_OnlyStartPages() {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var search = client.Search<Svedala.Public.Web.Models.Pages.StartPage>(x => x.QueryString("*").Index(IndexResolver.GetIndex()));

            Assert.NotNull(search);
            Assert.NotEmpty(search.Hits);
            Assert.All(search.Documents, x=> Assert.Equal(typeof(Svedala.Public.Web.Models.Pages.StartPage),x.GetOriginalType()));

        }

        [Fact]
        public void IndexProtectedPage() {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var page = contentLoader.Get<PageData>(new ContentReference(43));

            var result = client.Index(page, x => x.Index(IndexResolver.GetIndex()));

            Assert.True(result.IsValid);
        }

        [Fact]
        public void AddingCustomJsonConverter() {

            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            Assert.True(ContentIndexer.Instance.IndexingConventions.JsonConverters.Any(x=> x.GetType() == typeof(TestJsonConverter)));
            var result = client.Search<PageData>(s => s.Query(q => q.QueryString(qu => qu.Query("*"))));
            Assert.True(TestJsonConverter.TestIndexConverterUsed);



        }

    }
}

[InitializableModule]
public class TestSearchClientInitialization : IConfigurableModule
{
    public void Initialize(InitializationEngine context) {

    }

    public void Uninitialize(InitializationEngine context) {
        
    }

    public void ConfigureContainer(ServiceConfigurationContext context) {
        ContentIndexer.Instance.IndexingConventions.JsonConverters.Add(new TestJsonConverter());
    }

    
}



public class TestJsonConverter : JsonConverter
{
    public static bool TestIndexConverterUsed = false;
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        return null;
    }

    public override bool CanConvert(Type objectType) {
        TestIndexConverterUsed = true;
        return false;
    }
}
