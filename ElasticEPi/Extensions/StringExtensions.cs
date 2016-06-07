using System.Globalization;
using System.Text.RegularExpressions;

namespace ElasticEPi.Extensions {
    public static class StringExtensions {

        public static string ToCamelCase(this string s) {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            string camelCase = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            if (s.Length > 1)
                camelCase += s.Substring(1);

            return camelCase;
        }

        public static string StripHtml(this string input) {
            var tagsExpression = new Regex(@"</?.+?>");
            return tagsExpression.Replace(input, " ");
        }

    }
}
