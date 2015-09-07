using System.Linq;
using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
{
    public static class DynamicDictionaryExtensions
    {
        public static Parameters ToParameters(this DynamicDictionary parameters)
        {
            return new Parameters(parameters.Keys.Select(key => new Parameter(key, parameters[key].ToString())));
        }
    }
}
