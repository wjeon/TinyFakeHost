using System;
using System.Collections.Generic;
using System.Linq;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
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
    }
}