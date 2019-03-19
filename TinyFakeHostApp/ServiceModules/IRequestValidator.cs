using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.ServiceModules
{
    public interface IRequestValidator
    {
        FakeHttpResponse GetValidatedFakeResponse(Method method, string url, IEnumerable<KeyValuePair<string, StringValues>> query, IEnumerable<KeyValuePair<string, StringValues>> form, string body);
    }
}