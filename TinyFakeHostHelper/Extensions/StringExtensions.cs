using System.Collections.Generic;
using System.Linq;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<UrlParameter> ParseParameters(this string parameterString)
        {
            if (string.IsNullOrEmpty(parameterString))
                return new List<UrlParameter>();

            var parameters = parameterString.Split('&')
                .Select(urlParam => urlParam.Split('='))
                .Select(param => new UrlParameter(param[0], param[1]));

            return parameters;
        }
    }
}