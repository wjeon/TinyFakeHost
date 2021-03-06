﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FakeItEasy;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using TinyFakeHostApp.Persistence;
using TinyFakeHostApp.RequestResponse;
using TinyFakeHostApp.ServiceModules;

namespace TinyFakeHostApp.Tests.Unit
{
    [TestFixture]
    public class RequestValidatorTests
    {
        private RequestValidator _requestValidator;
        private IFakeRequestResponseRepository _fakeRequestResponseRepository;
        private const Method RequestedMethod = Method.POST;
        private const string RequestedPath = "/resourcePath";
        private const string RequestedUrlParams = "urlParam1=value1&urlParam2=value2";
        private const string RequestedFormParams = "formParam1=value1&formParam2=value2";
        private const string RequestedBody = "{\"RequestedBody\":\"Test Body\"}";

        [SetUp]
        public void SetUp()
        {
            _fakeRequestResponseRepository = A.Fake<IFakeRequestResponseRepository>();
            _requestValidator = new RequestValidator(_fakeRequestResponseRepository);
        }

        [TestCase(Method.GET, "/path", "/path", "urlParam1=value1&urlParam2=value2", "", "", "")]
        [TestCase(Method.POST, "/products/<<Anything>>", "/products/56839571", "", "formParam1=value1&productId=PRD<<anything>>", "", "formParam1=value1&productId=PRD397041843")]
        [TestCase(Method.POST, "/", "/", "", "", "{\"RequestedBody\":\"Test Body\"}", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase(Method.POST, "/", "/", "", "", "{\"RequestedBody\":\"DateTime: <<ANYTHING>>\"}", "{\"RequestedBody\":\"DateTime: 2016-06-09T14:25:42-07:00\"}")]
        public void When_fake_request_has_correct_value_then_GetValidatedFakeResponse_method_returns_fake_response
            (Method method, string requestedPath, string receivedPath, string urlParams, string formParams, string requestedBody, string receivedBody)
        {
            var fakeResponse = new FakeResponse
                {
                    Content = "{\"message\":\"OK\"}",
                    ContentType = "application/json",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            var fakeRequestResponses = CreateFakeRequestResponses(
                method.ToString(), requestedPath, urlParams, formParams, requestedBody,
                fakeResponse
            );
            A.CallTo(() => _fakeRequestResponseRepository.GetAll()).Returns(fakeRequestResponses);

            var response = _requestValidator.GetValidatedFakeResponse(
                method, receivedPath,
                ParseToKeyValuePairParameters(urlParams),
                ParseToKeyValuePairParameters(formParams),
                receivedBody
            );

            Assert.That(response.StatusCode, Is.EqualTo(fakeResponse.ToResponse(string.Empty).StatusCode));
            Assert.That(response.Body, Is.EqualTo(fakeResponse.Content));
            Assert.That(response.ContentType, Is.EqualTo(fakeResponse.ContentType));
        }

        [TestCase("POST", Method.POST, "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/resourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.POST, "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/resourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/wrongResourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/resourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/wrongResourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/resourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/wrongResourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", Method.GET, "/wrongResourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", Method.POST, "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "")]
        public void When_fake_request_has_one_or_more_incorrect_value_then_GetValidatedFakeResponse_method_returns_BadRequest_status(string expectedMethod, Method requestedMethod, string expectedPath, string expectedUrlParameters, string expectedFormParameters, string expectedBody)
        {
            var fakeRequestResponses = CreateFakeRequestResponses(
                expectedMethod, expectedPath, expectedUrlParameters, expectedFormParameters, expectedBody);
            A.CallTo(() => _fakeRequestResponseRepository.GetAll()).Returns(fakeRequestResponses);

            var response = _requestValidator.GetValidatedFakeResponse(
                requestedMethod, RequestedPath,
                ParseToKeyValuePairParameters(RequestedUrlParams),
                ParseToKeyValuePairParameters(RequestedFormParams),
                RequestedBody
            );

            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void GetValidatedFakeResponse_method_validates_by_last_created_fake_first()
        {
            var responseWithBadRequestStatus = new FakeResponse { StatusCode = System.Net.HttpStatusCode.BadRequest };
            var responseWithOkStatus = new FakeResponse();
            var emptyBody = string.Empty;

            var fakeRequestResponses = CreateFakeRequestResponses(
                RequestedMethod.ToString(), RequestedPath, RequestedUrlParams, RequestedFormParams, emptyBody, responseWithBadRequestStatus, new DateTime(2016, 5, 28)
            ).ToList();
            fakeRequestResponses.AddRange(CreateFakeRequestResponses(
                RequestedMethod.ToString(), RequestedPath, RequestedUrlParams, RequestedFormParams, emptyBody, responseWithOkStatus, new DateTime(2016, 5, 29)
            ));

            A.CallTo(() => _fakeRequestResponseRepository.GetAll()).Returns(fakeRequestResponses);

            var response = _requestValidator.GetValidatedFakeResponse(
                RequestedMethod, RequestedPath,
                ParseToKeyValuePairParameters(RequestedUrlParams),
                ParseToKeyValuePairParameters(RequestedFormParams),
                emptyBody
            );

            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        private static IEnumerable<FakeRequestResponse> CreateFakeRequestResponses(string expectedMethod, string expectedPath, string expectedUrlParameters, string expectedFormParameters, string expectedBody, FakeResponse fakeResponse = null, DateTime? created = null)
        {
            return new List<FakeRequestResponse>
                {
                    new FakeRequestResponse
                        {
                            FakeRequest = new FakeRequest
                                {
                                    Method = (Method)Enum.Parse(typeof(Method), expectedMethod),
                                    Path = expectedPath,
                                    UrlParameters = new Parameters(ParseParameters(expectedUrlParameters)),
                                    FormParameters = new Parameters(ParseParameters(expectedFormParameters)),
                                    Body = expectedBody
                                },
                            FakeResponse = fakeResponse,
                            Created = created.HasValue ? created.Value : DateTime.MinValue
                        }
                };
        }

        public static IEnumerable<Parameter> ParseParameters(string parameterString)
        {
            if (string.IsNullOrEmpty(parameterString))
                return new List<Parameter>();

            var parameters = parameterString.Split('&')
                .Select(urlParam => urlParam.Split('='))
                .Select(param => new Parameter(param[0], param[1]));

            return parameters;
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> ParseToKeyValuePairParameters(string parameterString)
        {
            if (string.IsNullOrEmpty(parameterString))
                return new List<KeyValuePair<string, StringValues>>();

            var parameters = new List<KeyValuePair<string, StringValues>>();

            foreach (var @params in parameterString.Split('&').Select(parameter => parameter.Split('=')))
            {
                parameters.Add(new KeyValuePair<string, StringValues>(@params[0], @params[1]));
            }

            return parameters;
        }
    }
}
