using System.Collections.Generic;
using NUnit.Framework;
using Nancy;
using TinyFakeHostHelper.RequestResponse;
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
            var headers = new Dictionary<string, string> { { "X-Test-A", "Test Heaser A" }, { "X-Test-B", "Test Heaser B" } };
            const string reasonPhrase = "Request successfully processed";

            var fakeRespone = new FakeResponse
            {
                Content = content,
                ContentType = contentType,
                Headers = headers,
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase = reasonPhrase
            };

            var convertedResponse = fakeRespone.ToNancyResponse();

            Response expectedNancyResponse = content;
            expectedNancyResponse.ContentType = contentType;
            expectedNancyResponse.Headers = headers;
            expectedNancyResponse.StatusCode = Nancy.HttpStatusCode.OK;
            expectedNancyResponse.ReasonPhrase = reasonPhrase;

            Assert.IsTrue(convertedResponse.IsEqualTo(expectedNancyResponse));
        }
    }
}
