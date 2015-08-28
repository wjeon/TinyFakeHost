using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using TinyFakeHostHelper.Messaging;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FakeRequestResponseRepositoryTests
    {
        [Test]
        public void It_adds_FakeRequestResponse_and_reads_all_added_FakeRequestResponses()
        {
            var fakeRequestResponseRepository = new FakeRequestResponseRepository();

            var firstFakeRequestResponse = CreateFakeRequestResponseWith("A", HttpStatusCode.OK);
            var secondFakeRequestResponse = CreateFakeRequestResponseWith("B", HttpStatusCode.BadRequest);

            fakeRequestResponseRepository.Add(firstFakeRequestResponse);
            fakeRequestResponseRepository.Add(secondFakeRequestResponse);

            var fakeRequestResponses = fakeRequestResponseRepository.GetAll();

            var expectedFakeRequestResponses = new List<FakeRequestResponse>
            {
                firstFakeRequestResponse, secondFakeRequestResponse
            };

            Assert.IsTrue(fakeRequestResponses.SequenceEqual(expectedFakeRequestResponses));
        }

        private static FakeRequestResponse CreateFakeRequestResponseWith(string key, HttpStatusCode statusCode, FakeMessageRequest messageRequest = null)
        {
            var fakeRequestResponse = new FakeRequestResponse
            {
                FakeRequest = new FakeRequest
                {
                    Path = "/" + key + "/products",
                    Parameters = new UrlParameters(new List<UrlParameter>
                    {
                        new UrlParameter("param-" + key + "-1", "value-" + key + "-1"),
                        new UrlParameter("param-" + key + "-2", "value-" + key + "-2")
                    })
                },
                FakeResponse = new FakeResponse
                {
                    Content = "content " + key,
                    ContentType = "application/json",
                    Headers = new Dictionary<string, string> {{"header-" + key, "value-" + key}},
                    ReasonPhrase = "reason phrase " + key,
                    StatusCode = statusCode
                },
                FakeMessageRequest = messageRequest
            };

            return fakeRequestResponse;
        }
    }
}
