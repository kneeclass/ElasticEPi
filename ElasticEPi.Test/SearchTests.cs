using ElasticEPi.Test.EPiServerInitialization;
using EPiServer.ServiceLocation;
using Nest;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using EPiServer.Core;
using Xunit;

namespace ElasticEPi.Test
{
    public class SearchTests : IClassFixture<EPiServerInitializer> {
        public SearchTests(EPiServerInitializer initializer) {
            var applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            initializer.Initialize(applicationPath);
        }

        [Fact]
        public void CanHandleSimpleFilter()
        {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();

            var search = client.Search<Svedala.Public.Web.Models.Pages.Base.BasePageData>(
            x => x.QueryString("*").Filter(t =>
                t.Term(y => y.LanguageBranch, "sv")));

            Assert.NotNull(search);
            Assert.NotEmpty(search.Hits);
            Assert.False(search.Documents.Any(x=> x.LanguageBranch != "sv"));

        }
        [Fact]
        public void CanHandleMultiFilter()
        {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();

            var result = client.Search<Svedala.Public.Web.Models.Pages.Base.BasePageData>(
            x => x.QueryString("naturgas lorem").Filter(t =>
                t.Term(y => y.LanguageBranch, "sv") && t.Term(g => g.ChangedBy, "epiadmin")));

            Assert.NotNull(result);
            Assert.NotEmpty(result.Hits);
            Assert.True(result.Documents.All(x=> x.ChangedBy == "epiadmin"));

        }

        [Fact]
        public void Returns_SearchResult()
        {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var search = client.Search<Svedala.Public.Web.Models.Pages.StartPage>(x => x.QueryString("*"));

            Assert.NotNull(search);
            Assert.NotEmpty(search.Hits);

        }

        [Fact]
        public async void CanSearchAsync() {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var result = await client.SearchAsync<PageData>(x =>
                x.Filter(y =>
                    y.Terms(h => h.ChangedBy, new[] {"epiadmin"})));


           Assert.NotNull(result);
           Assert.NotEmpty(result.Hits);

        }

        [Fact]
        public void CanSearchUsingQueryContainers() {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();

            QueryContainer query1 = new TermQuery {
                Field = "changedBy",
                Value = "epiadmin"
            };
            QueryContainer query2 = new TermQuery {
                Field = "languageBranch",
                Value = "sv"
            };

            var result = client.Search<PageData>(x=>x
                .Query(q => {
                    var container = query1 &= query2;
                    return container;
                })
            );

            Assert.NotNull(result);
            Assert.NotEmpty(result.Hits);

            Assert.True(result.Documents.All(x => x.ChangedBy == "epiadmin"));

        }


        [Fact]
        public void GetNoHitsOnProtectedPage() {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var securePageHit = client.Search<PageData>(s => s.Filter(f => f.Term(t => t.ContentLink, 43)));
            Assert.NotNull(securePageHit);
            Assert.Empty(securePageHit.Hits);

        }

        [Fact]
        public void GetNoHitsOnProtectedPageWithOtherAccess()
        {
            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(
            "epiadmin", "unittest"), new[] { "Bröd", "Smör" });
            var securePageHit = client.Search<PageData>(s => s.Filter(f => f.Term(t => t.ContentLink, 43)));
            Assert.NotNull(securePageHit);
            Assert.Empty(securePageHit.Hits);

        }

        [Fact]
        public void GetsResultOnProtectedPage() {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(
                "epiadmin", "unittest"), new []{ "WebAdmins","Foo","Bröd" });

            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var securePageHit = client.Search<PageData>(s => s.Filter(f => f.Term(t => t.ContentLink, 43)));

            Assert.NotNull(securePageHit);
            Assert.NotEmpty(securePageHit.Hits);
        }

        [Fact]
        public void ShouldNotGetPageThatsNotPublished() {

            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var notpublishedPage = client.Search<PageData>(s => s.Filter(f => f.Term(t => t.ContentLink, 44)));

            Assert.NotNull(notpublishedPage);
            Assert.Empty(notpublishedPage.Hits);

        }

        [Fact]
        public void ShouldNotGetPageThatStopPublishedHasPassed() {

            var client = ServiceLocator.Current.GetInstance<IElasticClient>();
            var notpublishedPage = client.Search<PageData>(s => s.Filter(f => f.Term(t => t.ContentLink, 45)));

            Assert.NotNull(notpublishedPage);
            Assert.Empty(notpublishedPage.Hits);

        }


    }
}
