using System.Linq;
using System.Net;
using FakeItEasy;
using NUnit.Framework;
using TinyFakeHostApp.Fakers;
using TinyFakeHostApp.Persistence;
using TinyFakeHostApp.RequestResponse;
using TinyFakeHostApp.Supports;
using TinyFakeHostApp.Tests.Unit.Extensions;

namespace TinyFakeHostApp.Tests.Unit
{
    [TestFixture]
    public class FluentFakerTests
    {
        private IFakeRequestResponseRepository _repository;
        private FluentFaker _fluentFaker;

        [SetUp]
        public void SetUp()
        {
            var dateTimeProvider = A.Fake<IDateTimeProvider>();
            _repository = new FakeRequestResponseRepository(dateTimeProvider);

            _fluentFaker = new FluentFaker(_repository);
        }

        [Test]
        public void When_fluent_faker_fakes_request_path_with_parameters_and_fakes_response_it_stores_fake_request_and_response_in_the_repository()
        {
            const string path = "/vendors/9876-5432-1098-7654/products";
            const string urlParameters = "type=desk";
            const string formParameters = "manufactureYear=2013";
            const string body = "{\"RequestedBody\":\"Test Body\"}";
            const string content = @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]";
            const string contentType = "application/json";
            const HttpStatusCode statusCode = HttpStatusCode.OK;
            const string reasonPhrase = "Good reason";

            var fakeResponse = CreateFakeResponse(content, contentType, statusCode, reasonPhrase);

            _fluentFaker
                .IfRequest(path)
                .WithMethod(Method.POST)
                .WithUrlParameters(urlParameters)
                .WithFormParameters(formParameters)
                .WithBody(body)
                .ThenReturn(fakeResponse);

            AssertThatFakeRequestAndResponseAreStored(Method.POST, path, urlParameters, formParameters, body, content, contentType, statusCode, reasonPhrase);
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

            AssertThatFakeRequestAndResponseAreStored(Method.GET, path, null, null, string.Empty, content, contentType, statusCode, reasonPhrase);
        }

        private void AssertThatFakeRequestAndResponseAreStored
            (Method method, string path, string urlParameters, string formParameters, string body, string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            var requestsResponses = _repository.GetAll();

            Assert.IsTrue(requestsResponses != null && requestsResponses.Count() == 1);

            var expectedRequestResponse = CreateFakeRequestResponse(method, path, urlParameters, formParameters, body, content, contentType, statusCode,
                                                                    reasonPhrase);

            Assert.IsTrue(requestsResponses.First().IsEqualTo(expectedRequestResponse));
        }

        private static FakeRequestResponse CreateFakeRequestResponse
            (Method method, string path, string urlParameters, string formParameters, string body, string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            return new FakeRequestResponse
            {
                FakeRequest = new FakeRequest { Method = method, Path = path, UrlParameters = ParseParameters(urlParameters), FormParameters = ParseParameters(formParameters), Body = body },
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
