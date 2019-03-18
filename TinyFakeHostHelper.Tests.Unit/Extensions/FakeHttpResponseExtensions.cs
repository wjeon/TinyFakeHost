using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.ServiceModules;

namespace TinyFakeHostHelper.Tests.Unit.Extensions
{
    public static class FakeHttpResponseExtensions
    {
        public static bool IsEqualTo(this FakeHttpResponse response, FakeHttpResponse otherResponse)
        {
            return response.Body == otherResponse.Body &&
                   response.ContentType == otherResponse.ContentType &&
                   response.Headers == otherResponse.Headers &&
                   response.StatusCode == otherResponse.StatusCode;
        }
    }
}
