using System;
using System.IO;
using ElasticEPi.Logging;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Logging;
using EPiServer.Web.Routing;

namespace ElasticEPi.Extensions
{
    public static class MediaDataExtensions {

        public static string ToBase64Content(this MediaData mediaData) {

            try {
                var blog = BlobFactory.Instance.GetBlob(mediaData.BinaryData.ID);
                using (var stream = blog.OpenRead()) {
                    using (var memoryStream = new MemoryStream()) {
                        stream.CopyTo(memoryStream);
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception exception) {
                Logger.WriteToLog($"MediaData content to ToBase64Content Failed, returning empty content. Content: ID={mediaData.ContentLink.ID} Name={mediaData.Name} Type={mediaData.GetOriginalType().Name}",Level.Error, exception);
                return string.Empty;
            }
        }



    }
}
