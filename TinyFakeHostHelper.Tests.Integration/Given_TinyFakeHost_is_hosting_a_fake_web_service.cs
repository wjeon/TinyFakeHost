﻿using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using RestSharp;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    class Given_TinyFakeHost_is_hosting_a_fake_web_service : TinyFakeHostTestBase
    {
        private TinyFakeHost _tinyFakeHost;
        private RequestResponseFaker _faker;

        [SetUp]
        public void Given()
        {
            _tinyFakeHost = new TinyFakeHost(BaseUri);

            _tinyFakeHost.Start();

            _faker = _tinyFakeHost.GetFaker();
        }

        [TestCase("/vendors/9876-5432-1098-7654/products", "type=desk&manufactureYear=2013", @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]", "application/json", "OK")]
        [TestCase("/invalidPath", "param=invalidParameter", @"{""message"":""Bad Request""}", "application/json", "BadRequest")]
        public void When_a_web_client_queries_the_fake_web_service_with_parameters_it_returns_a_fake_content(string resourcePath, string urlParameters, string responseContent, string contentType, string statusCode)
        {
            _faker.Fake(f => f
                .IfRequest(resourcePath)
                .WithParameters(urlParameters)
                .ThenReturn(new FakeResponse { ContentType = contentType, Content = responseContent, StatusCode = ParseHttpStatusCode(statusCode) })
            );

            var response = CallFakeService(resourcePath, urlParameters);

            Assert.AreEqual(responseContent, response.Content);
        }

        [TestCase("/helloWorld", "Hello world", "text/plain", "BadRequest")]
        [TestCase("/vendors/9876-5432-1098-7654/products", @"[{""id"":460173,""name"":""Product A"",""type"":""chair"",""manufactureYear"":2014},{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]", "application/json", "OK")]
        public void When_a_web_client_queries_the_fake_web_service_without_parameters_it_returns_a_fake_content(string resourcePath, string responseContent, string contentType, string statusCode)
        {
            _faker.Fake(f => f
                .IfRequest(resourcePath)
                .ThenReturn(new FakeResponse { ContentType = contentType, Content = responseContent, StatusCode = ParseHttpStatusCode(statusCode) })
            );

            var response = CallFakeService(resourcePath);

            Assert.AreEqual(responseContent, response.Content);
        }

        [TearDown]
        public void TearDown()
        {
            _tinyFakeHost.Stop();

            _tinyFakeHost.Dispose();
        }

        private IRestResponse CallFakeService(string resourcePath, string urlParameters = null)
        {
            var request = CreateRequest(resourcePath);

            AddParametersToRequest(request, urlParameters);

            var response = RestClient.Execute(request);
            return response;
        }

        private static void AddParametersToRequest(IRestRequest request, string urlParameters)
        {
            if (!string.IsNullOrEmpty(urlParameters))
                foreach (var param in urlParameters.Split('&').Select(parameter => parameter.Split('=')))
                    request.AddParameter(param[0], param[1]);
        }

        private static HttpStatusCode ParseHttpStatusCode(string statusCode)
        {
            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCode);
        }
    }
}
