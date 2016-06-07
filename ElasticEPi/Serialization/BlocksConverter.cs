using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;

namespace ElasticEPi.Serialization
{
    internal class BlocksConverter
    {

        public static string Convert(IContent content) {

            var properties = content.GetOriginalType()
                   .GetProperties(BindingFlags.Public |
                                  BindingFlags.Instance |
                                  BindingFlags.DeclaredOnly);
            var list = new List<string>();
            foreach (var property in properties) {
                if(property.GetCustomAttribute(typeof (ElasticEPiIgnoreAttribute)) != null) continue;
                var value = property.GetValue(content) as string;
                if (!string.IsNullOrWhiteSpace(value))
                    list.Add(value);
            }
            return string.Join(" ", list);


        }



    }
}
