using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.Extensions
{
    public static class StringExtensions
    {
        [Obsolete("Please use \"ParseParameters\" instead")]
        public static IEnumerable<Parameter> ParseUrlParameters(this string parameterString)
        {
            return ParseParameters(parameterString);
        }

        public static IEnumerable<Parameter> ParseParameters(this string parameterString)
        {
            if (string.IsNullOrEmpty(parameterString))
                return new List<Parameter>();

            var parameters = parameterString.Split('&')
                .Select(urlParam => urlParam.Split('='))
                .Select(param => new Parameter(param[0], param[1]));

            return parameters;
        }

        public static bool Matches(this string patternedValue, string value)
        {
            var pattern = "^" + Regex.Replace(Regex.Escape(patternedValue ?? string.Empty), "<<anything>>", "(.*)", RegexOptions.IgnoreCase) + "$";
            return Regex.IsMatch(value ?? string.Empty, pattern);
        }
    }
}