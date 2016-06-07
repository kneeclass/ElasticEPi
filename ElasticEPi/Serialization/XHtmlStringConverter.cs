using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ElasticEPi.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization {
    public class XHtmlStringConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            var xhtmlstring = value as XhtmlString;
            if (xhtmlstring == null) return;

            var str = string.Empty;

            foreach (var fragment in xhtmlstring.Fragments) {
                str += " "+CreateSearchableFragment(fragment);
            }

            writer.WriteValue(str);
        }


        private string CreateSearchableFragment(IStringFragment fragment) {

            if (fragment is StaticFragment) {
                return fragment.InternalFormat;
            }
            if (fragment is ContentFragment) {
                var contentFragment = (ContentFragment) fragment;
                if (contentFragment.ContentLink == ContentReference.EmptyReference) return string.Empty;
                if (contentFragment.GetContent() == null) return string.Empty;
                return BlocksConverter.Convert(contentFragment.GetContent());
            }

            return string.Empty;
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof (XhtmlString);

        }
    }


}
