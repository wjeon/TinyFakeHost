using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.ServiceModules
{
    public interface IRequestValidator
    {
        Response GetValidatedFakeResponse(Method method, Url url, DynamicDictionary query, DynamicDictionary form);
    }
}