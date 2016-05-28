using System.Linq;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit.Extensions
{
    public static class FakeRequestResponseExtensions
    {
        public static bool IsEqualTo(this FakeRequestResponse requestResponse, FakeRequestResponse otherRequestResponse)
        {
            return
                requestResponse.FakeRequest.Method == otherRequestResponse.FakeRequest.Method &&
                requestResponse.FakeRequest.Path == otherRequestResponse.FakeRequest.Path &&
                requestResponse.FakeRequest.UrlParameters.SequenceEqual(otherRequestResponse.FakeRequest.UrlParameters) &&
                requestResponse.FakeRequest.FormParameters.SequenceEqual(otherRequestResponse.FakeRequest.FormParameters) &&
                requestResponse.FakeRequest.Body == otherRequestResponse.FakeRequest.Body &&
                requestResponse.FakeResponse.Content == otherRequestResponse.FakeResponse.Content &&
                requestResponse.FakeResponse.ContentType == otherRequestResponse.FakeResponse.ContentType &&
                requestResponse.FakeResponse.StatusCode == otherRequestResponse.FakeResponse.StatusCode;
        }
    }
}