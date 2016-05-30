using System.Linq;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
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
        private IFakeRequestResponseRepository _repository;
        private ITinyFakeHostConfiguration _configuration;
        private FluentFaker _fluentFaker;

        [SetUp]
        public void SetUp()
        {
            _repository = new FakeRequestResponseRepository();
            _configuration = new TinyFakeHostConfiguration();

            _fluentFaker = new FluentFaker(_repository, _configuration);
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

        [Test]
        public void LastCreatedFakeId_property_returns_id_of_last_created_fake()
        {
            _fluentFaker.IfRequest("/pathForFirstFake").ThenReturn(new FakeResponse());
            _fluentFaker.IfRequest("/pathForSecondFake").ThenReturn(new FakeResponse());

            var lastCreatedFakeId = _repository.GetAll().ToList().Find(a => a.FakeRequest.Path == "/pathForSecondFake").Id;

            Assert.AreEqual(_fluentFaker.LastCreatedFakeId, lastCreatedFakeId);
        }

        [Test]
        public void DeleteAllFakes_method_calls_DeleteAll_method_in_FakeRequestResponseRepository()
        {
            var repository = MockRepository.GenerateStub<IFakeRequestResponseRepository>();
            _fluentFaker = new FluentFaker(repository, _configuration);

            _fluentFaker.DeleteAllFakes();

            repository.AssertWasCalled(r => r.DeleteAll());
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
