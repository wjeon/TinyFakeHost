using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FakeRequestResponseRepositoryTests
    {
        private FakeRequestResponseRepository _fakeRequestResponseRepository;

        [SetUp]
        public void SetUp()
        {
            _fakeRequestResponseRepository = new FakeRequestResponseRepository();
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
        public void When_FakeRequestResponse_with_existing_id_is_added_it_throws_unique_id_exception()
        {
            var fakeRequestResponse = new FakeRequestResponse();
            _fakeRequestResponseRepository.Add(fakeRequestResponse);

            Assert.Throws<UniqueIdException>(() =>
                _fakeRequestResponseRepository.Add(fakeRequestResponse)
            );
        }

        [Test]
        public void DeleteAll_method_deletes_all_stored_fakes()
        {
            var firstFakeRequestResponse = CreateFakeRequestResponseWith("A", HttpStatusCode.OK);
            var secondFakeRequestResponse = CreateFakeRequestResponseWith("B", HttpStatusCode.BadRequest);

            _fakeRequestResponseRepository.Add(firstFakeRequestResponse);
            _fakeRequestResponseRepository.Add(secondFakeRequestResponse);
            Assert.That(_fakeRequestResponseRepository.GetAll().Count(), Is.EqualTo(2));

            _fakeRequestResponseRepository.DeleteAll();

            Assert.That(_fakeRequestResponseRepository.GetAll().Count(), Is.EqualTo(0));
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
