using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.Supports;
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
            var dateTimeProvider = MockRepository.GenerateMock<IDateTimeProvider>();
            _repository = new FakeRequestResponseRepository(dateTimeProvider);
            _configuration = new TinyFakeHostConfiguration();

            _fluentFaker = new FluentFaker(_repository, _configuration);
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
        public void DeleteFakeById_method_calls_DeleteById_method_in_FakeRequestResponseRepository()
        {
            var id = Guid.NewGuid();
            var repository = MockRepository.GenerateStub<IFakeRequestResponseRepository>();
            _fluentFaker = new FluentFaker(repository, _configuration);

            _fluentFaker.DeleteFakeById(id);

            repository.AssertWasCalled(r => r.DeleteById(id));
        }

        [Test]
        public void When_delete_fake_by_last_created_fake_id_it_also_sets_the_stored_last_created_fake_id_to_null()
        {
            _fluentFaker.IfRequest("/pathForLastFake").ThenReturn(new FakeResponse());
            Assert.IsNotNull(_fluentFaker.LastCreatedFakeId);
            var lastCreatedFakeId = _fluentFaker.LastCreatedFakeId;

            _fluentFaker.DeleteFakeById(lastCreatedFakeId.Value);

            Assert.IsNull(_fluentFaker.LastCreatedFakeId);
        }

        public void DeleteAllFakes_method_calls_DeleteAll_method_in_FakeRequestResponseRepository()
        {
            var repository = MockRepository.GenerateStub<IFakeRequestResponseRepository>();
            _fluentFaker = new FluentFaker(repository, _configuration);

            _fluentFaker.DeleteAllFakes();

            repository.AssertWasCalled(r => r.DeleteAll());
        }
 
        [Test]
        public void When_delete_all_fakes_it_also_sets_the_stored_last_created_fake_id_to_null()
        {
            _fluentFaker.IfRequest("/pathForLastFake").ThenReturn(new FakeResponse());
            Assert.IsNotNull(_fluentFaker.LastCreatedFakeId);

            _fluentFaker.DeleteAllFakes();

            Assert.IsNull(_fluentFaker.LastCreatedFakeId);
        }

        [Test]
        public void DeleteLastCreatedFake_method_deletes_fake_by_stored_last_created_fake_id()
        {
            _fluentFaker.IfRequest("/pathForLastFake").ThenReturn(new FakeResponse());
            Assert.IsNotNull(StoredLastFake());

            _fluentFaker.DeleteLastCreatedFake();

            Assert.IsNull(StoredLastFake());
        }

        private FakeRequestResponse StoredLastFake()
        {
            return _repository.GetAll().ToList().Find(a => a.FakeRequest.Path == "/pathForLastFake");
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
