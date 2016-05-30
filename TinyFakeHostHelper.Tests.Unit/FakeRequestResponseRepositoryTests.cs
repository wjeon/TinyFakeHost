using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.Supports;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FakeRequestResponseRepositoryTests
    {
        private IDateTimeProvider _dateTimeProvider;
        private FakeRequestResponseRepository _fakeRequestResponseRepository;

        [SetUp]
        public void SetUp()
        {
            _dateTimeProvider = MockRepository.GenerateMock<IDateTimeProvider>();
            _fakeRequestResponseRepository = new FakeRequestResponseRepository(_dateTimeProvider);
        }

        [Test]
        public void It_adds_FakeRequestResponse_and_reads_all_added_FakeRequestResponses()
        {
            var firstFakeRequestResponse = CreateFakeRequestResponseWith("A", HttpStatusCode.OK);
            var secondFakeRequestResponse = CreateFakeRequestResponseWith("B", HttpStatusCode.BadRequest);

            _fakeRequestResponseRepository.Add(firstFakeRequestResponse);
            _fakeRequestResponseRepository.Add(secondFakeRequestResponse);

            var fakeRequestResponses = _fakeRequestResponseRepository.GetAll();

            var expectedFakeRequestResponses = new List<FakeRequestResponse>
            {
                firstFakeRequestResponse, secondFakeRequestResponse
            };

            Assert.IsTrue(fakeRequestResponses.SequenceEqual(expectedFakeRequestResponses));
        }

        [Test]
        public void It_adds_FakeRequestResponse_with_Created_property_sets_to_current_time()
        {
            var currentTime = new DateTimeOffset(2016, 5, 29, 6, 7, 21, TimeSpan.FromHours(-7));
            _dateTimeProvider.Stub(s => s.UtcNow).Return(currentTime);

            var fake = CreateFakeRequestResponseWith("A", HttpStatusCode.OK);
            _fakeRequestResponseRepository.Add(fake);

            var fakes = _fakeRequestResponseRepository.GetAll();
            Assert.AreEqual(fakes.First().Created, currentTime);
        }

        private static FakeRequestResponse CreateFakeRequestResponseWith(string key, HttpStatusCode statusCode)
        {
            var fakeRequestResponse = new FakeRequestResponse
            {
                FakeRequest = new FakeRequest
                {
                    Path = "/" + key + "/products",
                    UrlParameters = new Parameters(new List<Parameter>
                    {
                        new Parameter("param-" + key + "-1", "value-" + key + "-1"),
                        new Parameter("param-" + key + "-2", "value-" + key + "-2")
                    })
                },
                FakeResponse = new FakeResponse
                {
                    Content = "content " + key,
                    ContentType = "application/json",
                    Headers = new Dictionary<string, string> {{"header-" + key, "value-" + key}},
                    ReasonPhrase = "reason phrase " + key,
                    StatusCode = statusCode
                }
            };

            return fakeRequestResponse;
        }
    }
}
