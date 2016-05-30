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
            var fakeRequestResponse = AddFakeRequestResponse();

            Assert.Throws<UniqueIdException>(() =>
                _fakeRequestResponseRepository.Add(fakeRequestResponse)
            );
        }

        [Test]
        public void DeleteById_method_deletes_FakeRequestResponse_by_its_id()
        {
            var fakeRequestResponse = AddFakeRequestResponse();
            var id = fakeRequestResponse.Id;
            Assert.IsTrue(_fakeRequestResponseRepository.GetAll().Any(f => f.Id == id));

            _fakeRequestResponseRepository.DeleteById(id);
            Assert.IsFalse(_fakeRequestResponseRepository.GetAll().Any(f => f.Id == id));
        }

        private FakeRequestResponse AddFakeRequestResponse()
        {
            var fakeRequestResponse = new FakeRequestResponse();
            _fakeRequestResponseRepository.Add(fakeRequestResponse);
            return fakeRequestResponse;
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
