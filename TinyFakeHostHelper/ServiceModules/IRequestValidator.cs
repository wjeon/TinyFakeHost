using Nancy;

namespace TinyFakeHostHelper.ServiceModules
{
    public interface IRequestValidator
    {
        Response GetValidatedFakeResponse(Url url, DynamicDictionary query, DynamicDictionary form);
    }
}