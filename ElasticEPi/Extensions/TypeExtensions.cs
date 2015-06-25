using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticEPi.Extensions {
    internal static class TypeExtensions {

        public static IEnumerable<string> GetInheritancHierarchy(this Type type) {
            for (var current = type; current != null; current = current.BaseType)
                yield return current.ToSimpleAssemblyName();
        }

        public static string ToSimpleAssemblyName(this Type type) {
            var fullyQualifiedTypeName = type.AssemblyQualifiedName;
            if (string.IsNullOrEmpty(fullyQualifiedTypeName)) return string.Empty;
            var builder = new StringBuilder();
            var writingAssemblyName = false;
            var skippingAssemblyDetails = false;
            foreach (var current in fullyQualifiedTypeName) {
                switch (current) {
                    case '[':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        builder.Append(current);
                        break;
                    case ']':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        builder.Append(current);
                        break;
                    case ',':
                        if (!writingAssemblyName) {
                            writingAssemblyName = true;
                            builder.Append(current);
                        } else {
                            skippingAssemblyDetails = true;
                        }
                        break;
                    default:
                        if (!skippingAssemblyDetails)
                            builder.Append(current);
                        break;
                }
            }

            return builder.ToString();
        }


    }
}
