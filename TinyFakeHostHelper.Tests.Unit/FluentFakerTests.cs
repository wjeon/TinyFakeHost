using System.Linq;
using System.Net;
using NUnit.Framework;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.Tests.Unit.Extensions;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FluentFakerTests
    {
        private IFakeRequestResponseRepository _repository;
        private FluentFaker _fluentFaker;

        [SetUp]
        public void SetUp()
        {
            _repository = new FakeRequestResponseRepository();

            _fluentFaker = new FluentFaker(_repository);
        }

        [Test]
        public void When_fluent_faker_fakes_request_path_with_parameters_and_fakes_response_it_stores_fake_request_and_response_in_the_repository()
        {
            const string path = "/vendors/9876-5432-1098-7654/products";
            const string urlParameters = "type=desk";
            const string formParameters = "manufactureYear=2013";
            const string content = @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]";
            const string contentType = "application/json";
            const HttpStatusCode statusCode = HttpStatusCode.OK;
            const string reasonPhrase = "Good reason";

            var fakeResponse = CreateFakeResponse(content, contentType, statusCode, reasonPhrase);

            _fluentFaker
                .IfRequest(path)
                .WithUrlParameters(urlParameters)
                .WithFormParameters(formParameters)
                .ThenReturn(fakeResponse);

            AssertThatFakeRequestAndResponseAreStored(path, urlParameters, formParameters, content, contentType, statusCode, reasonPhrase);
        }

        [Test]
        public void When_fluent_faker_fakes_request_path_without_parameters_and_fakes_response_it_stores_fake_request_and_response_in_the_repository()
        {
            const string path = "/BadPath";
            const string content = "error";
            const string contentType = "text/plain";
            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            const string reasonPhrase = "Bad reason";

            var fakeResponse = CreateFakeResponse(content, contentType, statusCode, reasonPhrase);

            _fluentFaker
                .IfRequest(path)
                .ThenReturn(fakeResponse);

            AssertThatFakeRequestAndResponseAreStored(path, null, null, content, contentType, statusCode, reasonPhrase);
        }

        private void AssertThatFakeRequestAndResponseAreStored
            (string path, string urlParameters, string formParameters, string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            var requestsResponses = _repository.GetAll();

            Assert.IsTrue(requestsResponses != null && requestsResponses.Count() == 1);

            var expectedRequestResponse = CreateFakeRequestResponse(path, urlParameters, formParameters, content, contentType, statusCode,
                                                                    reasonPhrase);

            Assert.IsTrue(requestsResponses.First().IsEqualTo(expectedRequestResponse));
        }

        private static FakeRequestResponse CreateFakeRequestResponse
            (string path, string urlParameters, string formParameters, string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            return new FakeRequestResponse
            {
                FakeRequest = new FakeRequest { Path = path, UrlParameters = ParseParameters(urlParameters), FormParameters = ParseParameters(formParameters) },
                FakeResponse = CreateFakeResponse(content, contentType, statusCode, reasonPhrase)
            };
        }

        private static FakeResponse CreateFakeResponse(string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            return new FakeResponse
            {
                Content = content,
                ContentType = contentType,
                StatusCode = statusCode,
                ReasonPhrase = reasonPhrase
            };
        }

        private static Parameters ParseParameters(string urlParameterString)
        {
            return string.IsNullOrEmpty(urlParameterString)
                ? new Parameters()
                : new Parameters(
                    urlParameterString.Split('&')
                    .Select(urlParam => urlParam.Split('='))
                    .Select(param => new Parameter(param[0], param[1]))
                );
        }
    }
}
