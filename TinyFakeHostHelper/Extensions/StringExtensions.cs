using System.Collections.Generic;
using System.Linq;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<UrlParameter> ParseUrlParameters(this string urlParameterString)
        {
            var parameters = urlParameterString.Split('&')
                .Select(urlParam => urlParam.Split('='))
                .Select(param => new UrlParameter(param[0], param[1]));

            return parameters;
        }
    }
}