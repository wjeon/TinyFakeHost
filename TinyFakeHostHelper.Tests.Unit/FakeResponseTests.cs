using System.Collections.Generic;
using NUnit.Framework;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.ServiceModules;
using TinyFakeHostHelper.Tests.Unit.Extensions;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FakeResponseTests
    {
        [Test]
        public void ToNancyResponse_converts_FakeResponse_to_nancy_response_and_returns()
        {
            const string content = "Hello world";
            const string contentType = "application/json";
            var headers = new Dictionary<string, string> { { "X-Test-A", "Test Header A" }, { "X-Test-B", "Test Header B" } };
            const string reasonPhrase = "Request successfully processed";

            var fakeRespone = new FakeResponse
            {
                Content = content,
                ContentType = contentType,
                Headers = headers,
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase = reasonPhrase
            };

            var convertedResponse = fakeRespone.ToResponse(string.Empty);

            var expectedHttpResponse = new FakeHttpResponse
            {
                Body = content,
                ContentType = contentType,
                Headers = headers,
                StatusCode = (int)HttpStatusCode.OK
            };

            Assert.IsTrue(convertedResponse.IsEqualTo(expectedHttpResponse));
        }
    }
}
