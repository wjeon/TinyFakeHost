using System.Linq;
using System.Net;
using NUnit.Framework;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.Tests.Unit.Extensions;
using TinyFakeHostHelper.Tests.Unit.Helpers;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FluentFakerTests
    {
        private TinyIoCContainer _container;
        private IFakeRequestResponseRepository _repository;
        private ITinyFakeHostConfiguration _configuration;
        private FluentFaker _fluentFaker;

        [SetUp]
        public void SetUp()
        {
            _container = new TinyIoCContainer();
            _repository = new FakeRequestResponseRepository();
            _configuration = new TinyFakeHostConfiguration();
            _container.Register(_repository);
            _container.Register(_configuration);

            _fluentFaker = new FluentFaker(_container);
        }

        [Test]
        public void When_fluent_faker_fakes_request_path_with_parameters_and_fakes_response_it_stores_fake_request_and_response_in_the_repository()
        {
            const string path = "/vendors/9876-5432-1098-7654/products";
            const string parameters = "type=desk&manufactureYear=2013";
            const string content = @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]";
            const string contentType = "application/json";
            const HttpStatusCode statusCode = HttpStatusCode.OK;
            const string reasonPhrase = "Good reason";

            var fakeResponse = CreateFakeResponse(content, contentType, statusCode, reasonPhrase);

            _fluentFaker
                .IfRequest(path)
                .WithParameters(parameters)
                .ThenReturn(fakeResponse);

            AssertThatFakeRequestAndResponseAreStored(path, parameters, content, contentType, statusCode, reasonPhrase);
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

            AssertThatFakeRequestAndResponseAreStored(path, null, content, contentType, statusCode, reasonPhrase);
        }

        [TestCase("/vendors")]
        [TestCase("/vendors/9876-5432-1098-7654")]
        public void When_fluent_faker_fakes_request_path_with_number_of_segments_that_is_equal_or_less_than_the_number_configured_it_does_not_throw_exception(string requestPath)
        {
            AppSettingHelper.AddAppSettingInMemory("MaximumNumberOfPathSegments", "2");

            Assert.DoesNotThrow(() => _fluentFaker.IfRequest(requestPath));

            AppSettingHelper.RemoveAppSettingInMemory("MaximumNumberOfPathSegments");
        }

        [Test]
        public void When_fluent_faker_fakes_request_path_with_number_of_segments_that_is_more_than_the_number_configured_it_throws_MaximumNumberOfUrlPathSegmentsException()
        {
            AppSettingHelper.AddAppSettingInMemory("MaximumNumberOfPathSegments", "2");

            Assert.Throws<MaximumNumberOfUrlPathSegmentsException>(() => _fluentFaker.IfRequest("/vendors/9876-5432-1098-7654/products"));

            AppSettingHelper.RemoveAppSettingInMemory("MaximumNumberOfPathSegments");
        }

        private void AssertThatFakeRequestAndResponseAreStored
            (string path, string parameters, string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            var requestsResponses = _repository.GetAll();

            Assert.IsTrue(requestsResponses != null && requestsResponses.Count() == 1);

            var expectedRequestResponse = CreateFakeRequestResponse(path, parameters, content, contentType, statusCode,
                                                                    reasonPhrase);

            Assert.IsTrue(requestsResponses.First().IsEqualTo(expectedRequestResponse));
        }

        private static FakeRequestResponse CreateFakeRequestResponse
            (string path, string parameters, string content, string contentType, HttpStatusCode statusCode, string reasonPhrase)
        {
            return new FakeRequestResponse
            {
                FakeRequest = new FakeRequest { Path = path, Parameters = ParseUrlParameters(parameters) },
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

        private static UrlParameters ParseUrlParameters(string urlParameterString)
        {
            return string.IsNullOrEmpty(urlParameterString)
                ? new UrlParameters()
                : new UrlParameters(
                    urlParameterString.Split('&')
                    .Select(urlParam => urlParam.Split('='))
                    .Select(param => new UrlParameter(param[0], param[1]))
                );
        }
    }
}
