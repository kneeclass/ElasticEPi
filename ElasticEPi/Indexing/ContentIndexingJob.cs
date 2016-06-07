using System;
using System.Threading;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Newtonsoft.Json;

namespace ElasticEPi.Indexing
{
    [ScheduledPlugIn(Description = "Indexes EPiServer content into ElasticSearch",DisplayName = "ElasticEPi Indexing job")]
    public class ContentIndexingJob : ScheduledJobBase {

        private static readonly object ContentIndexingJobLock = new object();
        private readonly SiteDefinitionRepository _siteDefinitionRepository;

        public ContentIndexingJob() {
            _siteDefinitionRepository = ServiceLocator.Current.GetInstance<SiteDefinitionRepository>();
        }


        public override string Execute() {

            var items = 0;

            if (!Monitor.TryEnter(ContentIndexingJobLock))
                throw new NotSupportedException("The ContentIndexingJob is already running");
            try {

                foreach (var siteDefinition in _siteDefinitionRepository.List()) {
                    SiteDefinition.Current = siteDefinition;
                    var siteContentIndexer = new SiteContentIndexer();

                    items += siteContentIndexer.Start(StatusUpdate);
                }
            }
            catch (JsonWriterException e) {
                return "[FAILED]: " + e.Message + "  " + e.Path;
            }
            catch (Exception e) {
                return "[FAILED]: " + e.Message + "  " + e.StackTrace;
            }
            finally {
                Monitor.Exit(ContentIndexingJobLock);
            }

            return "[OK] - " + items;
        }

        private void StatusUpdate(string str) {
            OnStatusChanged(str);   
        }

    }
}
