using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElasticEPi.Exceptions;
using ElasticEPi.Logging;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Nest;

namespace ElasticEPi.Indexing
{
    public class ContentIndexer {

        private static IElasticClient Client => _client ?? (_client = ServiceLocator.Current.GetInstance<IElasticClient>());
        private static IElasticClient _client;
        private static IndexingConventions _conventions;
        private static ContentIndexer _instance;
        private static readonly object LockObj = new object();

        public IndexingConventions IndexingConventions => _conventions ?? (_conventions = new IndexingConventions());

        private ContentIndexer() { }

        public static ContentIndexer Instance
        {
            get {
                if (_instance == null) {
                    lock (LockObj) {
                        if (_instance == null)
                            _instance = new ContentIndexer();
                    }
                }
                return _instance;
            }
        }

        public IIndexResponse Index(IContent content) {
            try {
                var response = Client.Index(content, x => x.
                    Index(IndexResolver.GetIndex()).
                    Id(content.ContentLink.ID)
                );
                var onIndexed = content as IOnIndexed;
                if(onIndexed != null) onIndexed.OnIndexed();
                

                if (response.IsValid == false) throw new IndexException {
                    ServerError = response.ServerError,
                    RequestInformation = response.RequestInformation
                };
                Logger.WriteToLog($"Indexed content: Name={content.Name} Id={content.ContentLink.ID} Type={content.GetOriginalType().Name}. Elasticsearch response: Created={response.Created} Version={response.Version} Id={response.Id} Type={response.Type}");
                return response;
            }
            catch (IndexException exception) {
                Logger.WriteToLog($"Indexing content failed: Name={content.Name} Id={content.ContentLink.ID} Type={content.GetOriginalType().Name} Exception details: Request={Encoding.UTF8.GetString(exception.RequestInformation.Request)} Response={Encoding.UTF8.GetString(exception.RequestInformation.ResponseRaw)}"  ,Level.Error,exception);
            }
            catch (Exception exception) {
                Logger.WriteToLog("Unknown exception occured during indexing of content", Level.Error, exception);
            }
            return null;
        }

        public IBulkResponse Index(IEnumerable<IContent> contents) {
            try {
                if (!contents.Any()) return null;
                var bulkResponse = Client.IndexMany(contents, IndexResolver.GetIndex());
                if (bulkResponse.IsValid == false) throw new BulkIndexException {BulkResponse = bulkResponse};

                Logger.WriteToLog(
                    $"Bulk index: NumberOfContents={contents.Count()} Elasticsearch response: items= {string.Join(", ", bulkResponse.Items.Select(x => x.Id))}");
                return bulkResponse;
            }
            catch (BulkIndexException exception) {
                Logger.WriteToLog(
                    $"Bulk index partially failed: NumberOfContents={contents.Count()} Elasticsearch: items= {string.Join(", ", exception.BulkResponse.Items.Select(x => x.Id))} ItemsWithErrors={string.Join("| ", exception.BulkResponse.ItemsWithErrors.Select(x => "Id:"+x.Id+" Error:"+x.Error))}",Level.Error);
                return exception.BulkResponse;
            }
            catch (Exception exception) {
                Logger.WriteToLog("Unknown exception occured during bulk indexing of content", Level.Error, exception);
            }
            return null;
        }


        public IDeleteResponse Delete(IContent content) {
            try {
                var deleteResponse = Client.Delete(content, x => x.Index(IndexResolver.GetIndex()));
                if (deleteResponse.IsValid == false) throw new DeleteException {DeleteResponse = deleteResponse };
                Logger.WriteToLog($"Deleted content Name={content.Name} Id={content.ContentLink.ID} Type={content.GetOriginalType().Name} Elasticsearch response: Found={deleteResponse.Found} Id={deleteResponse.Id} Type={deleteResponse.Type}");
                return deleteResponse;
            }
            catch (DeleteException exception) {
                Logger.WriteToLog($"Delete content failed: Name={content.Name} Id={content.ContentLink.ID} Type={content.GetOriginalType().Name} Elasticsearch: Error={exception.DeleteResponse.ServerError.Error}",Level.Error);
            }
            catch (Exception exception)
            {
                Logger.WriteToLog("Unknown exception occured during delete indexing of content", Level.Error, exception);
            }
            return null;
        }

        


    }
}
