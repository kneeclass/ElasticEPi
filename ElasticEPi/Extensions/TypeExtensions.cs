using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElasticEPi.Extensions {
    internal static class TypeExtensions {

        public static IEnumerable<string> GetInheritancHierarchy(this Type type) {
            var retval = new List<string>();
            for (var current = type; current != null; current = current.BaseType) {
                retval.Add(current.ToSimpleAssemblyName());
                retval.AddRange(current.GetInterfaces().Select(x=> x.ToSimpleAssemblyName()));
            }
            return retval.Distinct();
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

        public static bool IsGenericTypeOf(this Type t, Type genericDefinition) {
            Type[] parameters = null;
            return IsGenericTypeOf(t, genericDefinition, out parameters);
        }

        public static bool IsGenericTypeOf(this Type t, Type genericDefinition, out Type[] genericParameters) {
            genericParameters = new Type[] { };
            if (!genericDefinition.IsGenericType) {
                return false;
            }

            var isMatch = t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
            if (!isMatch && t.BaseType != null) {
                isMatch = IsGenericTypeOf(t.BaseType, genericDefinition, out genericParameters);
            }
            if (!isMatch && genericDefinition.IsInterface && t.GetInterfaces().Any()) {
                foreach (var i in t.GetInterfaces()) {
                    if (i.IsGenericTypeOf(genericDefinition, out genericParameters)) {
                        isMatch = true;
                        break;
                    }
                }
            }

            if (isMatch && !genericParameters.Any()) {
                genericParameters = t.GetGenericArguments();
            }
            return isMatch;
        }




    }
}
