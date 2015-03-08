using System.Linq;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit.Extensions
{
    public static class FakeRequestResponseExtensions
    {
        public static bool IsEqualTo(this FakeRequestResponse requestResponse, FakeRequestResponse otherRequestResponse)
        {
            return
                requestResponse.FakeRequest.Path == otherRequestResponse.FakeRequest.Path &&
                requestResponse.FakeRequest.Parameters.SequenceEqual(otherRequestResponse.FakeRequest.Parameters) &&
                requestResponse.FakeResponse.Content == otherRequestResponse.FakeResponse.Content &&
                requestResponse.FakeResponse.ContentType == otherRequestResponse.FakeResponse.ContentType &&
                requestResponse.FakeResponse.StatusCode == otherRequestResponse.FakeResponse.StatusCode;
        }
    }
}