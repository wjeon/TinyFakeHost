using System.Linq;
using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
{
    public static class DynamicDictionaryExtensions
    {
        public static UrlParameters ToParameters(this DynamicDictionary parameters)
        {
            return new UrlParameters(parameters.Keys.Select(key => new UrlParameter(key, parameters[key].ToString())));
        }
    }
}
