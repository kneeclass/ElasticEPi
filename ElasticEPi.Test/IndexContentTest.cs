using ElasticEPi.Test.EPiServerInitialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticEPi.Indexing;
using Xunit;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Nest;

namespace ElasticEPi.Test
{
    public class IndexContentTest : IClassFixture<EPiServerInitializer> {
        
        public IndexContentTest(EPiServerInitializer initializer) {
            var applicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            initializer.Initialize(applicationPath);
        }
        [Fact]
        public void IndexContentSv() {
            var contentIndexerJob = new ContentIndexingJob();
            Assert.StartsWith("[OK]",contentIndexerJob.Execute());

        }
        [Fact]
        public void IndexFiles() {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var mediaDatas = contentLoader.GetDescendents(SiteDefinition.Current.GlobalAssetsRoot).ToList();
            Assert.NotEmpty(mediaDatas);

            foreach (var mediaData in mediaDatas) {
                MediaData outdata;
                if (contentLoader.TryGet(mediaData, out outdata)) {
                    var response = ContentIndexer.Instance.Index(outdata);
                    Assert.True(response.IsValid);
                }
            }
        }
        [Fact]
        public void IndexInterestingPage() {

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

            var page = contentLoader.Get<PageData>(new ContentReference(28));


            var response = ContentIndexer.Instance.Index(page);


            Assert.True(response.IsValid);
        }


        [Fact]
        public void BulkIndexTest() {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var contentReferences = contentLoader.GetDescendents(ContentReference.RootPage).ToList();
            var contents = contentLoader.GetItems(contentReferences,CultureInfo.GetCultureInfo("sv"));
            var response = ContentIndexer.Instance.Index(contents);

            Assert.NotNull(response);
            Assert.Empty(response.ItemsWithErrors ?? Enumerable.Empty<BulkOperationResponseItem>());


        }

        [Fact]
        public void DeleteContent() {

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var startPage = contentLoader.Get<IContent>(ContentReference.StartPage);

            var deletedPage = ContentIndexer.Instance.Delete(startPage);
            Assert.True(deletedPage.Found);

            var reIndexedPage = ContentIndexer.Instance.Index(startPage);
            Assert.True(reIndexedPage.Created);
            Assert.True(reIndexedPage.IsValid);
        }







    }
}
