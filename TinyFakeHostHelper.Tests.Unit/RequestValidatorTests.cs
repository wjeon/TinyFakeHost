using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nancy;
using Rhino.Mocks;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.ServiceModules;
using TinyFakeHostHelper.Tests.Unit.Extensions;

namespace TinyFakeHostHelper.Tests.Unit
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
            _fakeRequestResponseRepository = MockRepository.GenerateStub<IFakeRequestResponseRepository>();
            _requestValidator = new RequestValidator(_fakeRequestResponseRepository);
        }

        [Test]
        public void When_fake_request_has_correct_value_then_GetValidatedFakeResponse_method_returns_fake_response()
        {
            var fakeResponse = new FakeResponse
                {
                    Content = "{\"message\":\"OK\"}",
                    ContentType = "application/json",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            var fakeRequestResponses = CreateFakeRequestResponses(
                RequestedMethod.ToString(), RequestedPath, RequestedUrlParams, RequestedFormParams, RequestedBody,
                fakeResponse
            );
            _fakeRequestResponseRepository.Stub(s => s.GetAll()).Return(fakeRequestResponses);

            var response = _requestValidator.GetValidatedFakeResponse(
                RequestedMethod, new Url { Path = RequestedPath },
                DynamicDictionaryParseParameters(RequestedUrlParams),
                DynamicDictionaryParseParameters(RequestedFormParams),
                RequestedBody
            );

            Assert.That(response.StatusCode, Is.EqualTo(fakeResponse.ToNancyResponse().StatusCode));
            Assert.That(response.Content(), Is.EqualTo(fakeResponse.Content));
            Assert.That(response.ContentType, Is.EqualTo(fakeResponse.ContentType));
        }

        [TestCase("POST", "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/resourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/resourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/wrongResourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/resourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/wrongResourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/resourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/wrongResourcePath", "urlParam1=value1&urlParam2=value2", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/wrongResourcePath", "urlParam=incorrectValue", "formParam1=value1&formParam2=value2", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("GET", "/wrongResourcePath", "urlParam=incorrectValue", "formParam=incorrectValue", "{\"RequestedBody\":\"Test Body\"}")]
        [TestCase("POST", "/resourcePath", "urlParam1=value1&urlParam2=value2", "formParam1=value1&formParam2=value2", "")]
        public void When_fake_request_has_one_or_more_incorrect_value_then_GetValidatedFakeResponse_method_returns_BadRequest_status(string expectedMethod, string expectedPath, string expectedUrlParameters, string expectedFormParameters, string expectedBody)
        {
            var fakeRequestResponses = CreateFakeRequestResponses(
                expectedMethod, expectedPath, expectedUrlParameters, expectedFormParameters, expectedBody);
            _fakeRequestResponseRepository.Stub(s => s.GetAll()).Return(fakeRequestResponses);

            var response = _requestValidator.GetValidatedFakeResponse(
                RequestedMethod, new Url { Path = RequestedPath },
                DynamicDictionaryParseParameters(RequestedUrlParams),
                DynamicDictionaryParseParameters(RequestedFormParams),
                RequestedBody
            );

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        private static IEnumerable<FakeRequestResponse> CreateFakeRequestResponses(string expectedMethod, string expectedPath, string expectedUrlParameters, string expectedFormParameters, string expectedBody, FakeResponse fakeResponse = null)
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
                                FakeResponse = fakeResponse
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

        public static DynamicDictionary DynamicDictionaryParseParameters(string parameterString)
        {
            if (string.IsNullOrEmpty(parameterString))
                return new DynamicDictionary();

            var parameters = new DynamicDictionary();

            foreach (var @params in parameterString.Split('&').Select(parameter => parameter.Split('=')))
            {
                parameters.Add(@params[0], @params[1]);
            }

            return parameters;
        }
    }
}
