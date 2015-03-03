﻿using System.Linq;
using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    class Given_TinyFakeHost_is_hosting_a_fake_web_service : TinyFakeHostTestBase
    {
        private TinyFakeHost _tinyFakeHost;

        [SetUp]
        public void Given()
        {
            _tinyFakeHost = new TinyFakeHost(BaseUri);

            _tinyFakeHost.Start();
        }

        [TestCase("/helloWorld", "", "Hello world")]
        [TestCase("/vendors/9876-5432-1098-7654/products", "type=desk&manufactureYear=2013", @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]")]
        [TestCase("/vendors/9876-5432-1098-7654/products", "", @"[{""id"":460173,""name"":""Product A"",""type"":""chair"",""manufactureYear"":2014},{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]")]
        public void When_a_web_client_queries_the_fake_web_service_it_returns_a_fake_content(string resourcePath, string urlParameters, string responseContent)
        {
            var request = CreateRequest(resourcePath);

            AddParametersToRequest(urlParameters, request);

            var response = RestClient.Execute(request);

            Assert.AreEqual(responseContent, response.Content);
        }

        [TearDown]
        public void TearDown()
        {
            _tinyFakeHost.Stop();

            _tinyFakeHost.Dispose();
        }

        private static void AddParametersToRequest(string urlParameters, IRestRequest request)
        {
            if (!string.IsNullOrEmpty(urlParameters))
                foreach (var param in urlParameters.Split('&').Select(parameter => parameter.Split('=')))
                    request.AddParameter(param[0], param[1]);
        }
    }
}