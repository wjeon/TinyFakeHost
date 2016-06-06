using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using RestSharp;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.RequestResponse;
using Method = RestSharp.Method;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    class Given_TinyFakeHost_is_hosting_a_fake_web_service : TinyFakeHostTestBase
    {
        private TinyFakeHost _tinyFakeHost;
        private RequestResponseFaker _faker;
        private const string ResourcePath = "/resourcePath";
        private const string UrlParameter = "param=value";

        [SetUp]
        public void Given()
        {
            _tinyFakeHost = new TinyFakeHost(BaseUri);

            _tinyFakeHost.Start();

            _faker = _tinyFakeHost.GetFaker();
        }

        [TestCase("GET", "")]
        [TestCase("DELETE", "")]
        [TestCase("OPTIONS", "")]
        [TestCase("PATCH", "manufactureYear=2013")]
        [TestCase("POST", "manufactureYear=2013")]
        [TestCase("PUT", "manufactureYear=2013")]
        public void When_a_web_client_queries_the_fake_web_service_with_different_methods_it_returns_a_fake_content_correctly(string requestMethod, string formParameters)
        {
            const string responseContent = @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]";
            const string resourcePath = "/vendors/9876-5432-1098-7654/products";
            const string urlParameters = "type=desk";

            _faker.Fake(f => f
                .IfRequest(resourcePath)
                .WithMethod(TinyFakeHostMethod(requestMethod))
                .WithUrlParameters(urlParameters)
                .WithFormParameters(formParameters)
                .ThenReturn(new FakeResponse { ContentType = "application/json", Content = responseContent, StatusCode = ParseHttpStatusCode("OK") })
            );

            var request = new RestRequest(resourcePath, RestSharpMethod(requestMethod));

            AddUrlParametersToRequest(request, urlParameters);
            AddFormParametersToRequest(request, formParameters);

            _tinyFakeHost.RequestedQueryPrint = true;

            var response = RestClient.Execute(request);

            Assert.AreEqual(responseContent, response.Content);
        }

        private static RequestResponse.Method TinyFakeHostMethod(string method)
        {
            return (RequestResponse.Method)Enum.Parse(typeof(RequestResponse.Method), method);
        }

        private static Method RestSharpMethod(string method)
        {
            return (Method)Enum.Parse(typeof(Method), method);
        }

        [TestCase("/vendors/9876-5432-1098-7654/products", "type=desk&manufactureYear=2013", @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]", "application/json", "OK")]
        [TestCase("/invalidPath", "param=invalidParameter", @"{""message"":""Bad Request""}", "application/json", "BadRequest")]
        public void When_a_web_client_queries_the_fake_web_service_with_parameters_it_returns_a_fake_content(string resourcePath, string urlParameters, string responseContent, string contentType, string statusCode)
        {
            _faker.Fake(f => f
                .IfRequest(resourcePath)
                .WithUrlParameters(urlParameters)
                .ThenReturn(new FakeResponse { ContentType = contentType, Content = responseContent, StatusCode = ParseHttpStatusCode(statusCode) })
            );

            _tinyFakeHost.RequestedQueryPrint = true;

            var response = CallFakeService(resourcePath, urlParameters);

            Assert.AreEqual(responseContent, response.Content);
        }

        [TestCase("/", @"{""message"":""This is the root of the site""]", "application/json", "OK")]
        [TestCase("/helloWorld", "Hello world", "text/plain", "BadRequest")]
        [TestCase("/vendors/9876-5432-1098-7654/products", @"[{""id"":460173,""name"":""Product A"",""type"":""chair"",""manufactureYear"":2014},{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]", "application/json", "OK")]
        public void When_a_web_client_queries_the_fake_web_service_without_parameters_it_returns_a_fake_content(string resourcePath, string responseContent, string contentType, string statusCode)
        {
            _faker.Fake(f => f
                .IfRequest(resourcePath)
                .ThenReturn(new FakeResponse { ContentType = contentType, Content = responseContent, StatusCode = ParseHttpStatusCode(statusCode) })
            );

            _tinyFakeHost.RequestedQueryPrint = true;

            var response = CallFakeService(resourcePath);

            Assert.AreEqual(responseContent, response.Content);
        }

        [Test]
        public void When_a_web_client_queries_the_fake_web_service_with_long_sleep_time_it_fakes_long_process_time()
        {
            const string resourcePath = "/timeout";

            _faker.Fake(f => f
                .IfRequest(resourcePath)
                .ThenReturn(new FakeResponse{
                    ContentType = "application/json",
                    Content = @"{""message"":""Request Timeout""}",
                    StatusCode = HttpStatusCode.RequestTimeout,
                    MillisecondsSleep = 6000
                })
            );

            var response = CallFakeService(resourcePath, 3000);

            Assert.AreEqual("The operation has timed out", response.ErrorMessage);
        }

        [Test]
        public void When_a_web_client_queries_the_fake_web_service_the_requested_query_is_stored()
        {
            CallFakeService(ResourcePath, UrlParameter);

            var requestedQueries = _tinyFakeHost.GetRequestedQueries();

            Assert.IsTrue(requestedQueries.Any(a => a.Path == ResourcePath && a.UrlParameters.ToString() == UrlParameter));
        }

        [Test]
        public void When_fake_host_asserts_requested_query_correctly_with_resource_path_and_parameter_it_does_not_throw_exception()
        {
            CallFakeService(ResourcePath, UrlParameter);

            var asserter = _tinyFakeHost.GetAsserter();

            Assert.DoesNotThrow(() =>
                asserter.Assert(a => a
                    .Resource(ResourcePath)
                    .WithUrlParameters(UrlParameter)
                    .WasRequested()
                )
            );
        }

        [Test]
        public void When_fake_host_asserts_requested_query_correctly_with_resource_path_only_it_does_not_throw_exception()
        {
            CallFakeService(ResourcePath);

            var asserter = _tinyFakeHost.GetAsserter();

            Assert.DoesNotThrow(() =>
                asserter.Assert(a => a
                    .Resource(ResourcePath)
                    .WasRequested()
                )
            );
        }

        [Test]
        public void When_fake_host_asserts_requested_query_correctly_with_method_and_resource_path_and_form_parameter_it_does_not_throw_exception()
        {
            CallFakeService(Method.PUT, ResourcePath, null, "param=value");

            var asserter = _tinyFakeHost.GetAsserter();

            Assert.DoesNotThrow(() =>
                asserter.Assert(a => a
                    .Resource(ResourcePath)
                    .WithMethod(RequestResponse.Method.PUT)
                    .WithFormParameters("param=value")
                    .WasRequested()
                )
            );
        }

        [Test]
        public void When_fake_host_asserts_requested_query_incorrectly_with_wrong_resource_path_it_throws_assertion_exception()
        {
            CallFakeService(ResourcePath);

            var asserter = _tinyFakeHost.GetAsserter();

            Assert.Throws<Exceptions.AssertionException>(() =>
                asserter.Assert(a => a
                    .Resource("/wrongResourcePath")
                    .WasRequested()
                )
            );
        }

        [Test]
        public void When_fake_host_asserts_requested_query_incorrectly_with_wrong_parameter_it_throws_assertion_exception()
        {
            CallFakeService(ResourcePath, UrlParameter);

            var asserter = _tinyFakeHost.GetAsserter();

            Assert.Throws<Exceptions.AssertionException>(() =>
                asserter.Assert(a => a
                    .Resource(ResourcePath)
                    .WithUrlParameters("param=wrong+parameter")
                    .WasRequested()
                )
            );
        }

        [TearDown]
        public void TearDown()
        {
            _tinyFakeHost.Stop();

            _tinyFakeHost.Dispose();
        }

        private IRestResponse CallFakeService(string resourcePath, int timeout)
        {
            return CallFakeService(Method.GET, resourcePath, null, null, timeout);
        }

        private IRestResponse CallFakeService(string resourcePath, string urlParameters = null, string formParameters = null, int timeout = 0)
        {
            return CallFakeService(Method.GET, resourcePath, urlParameters, formParameters, timeout);
        }

        private IRestResponse CallFakeService(Method method, string resourcePath, string urlParameters = null, string formParameters = null, int timeout = 0)
        {
            var request = CreateRequest(resourcePath, method);

            AddUrlParametersToRequest(request, urlParameters);

            AddFormParametersToRequest(request, formParameters);

            RestClient.Timeout = timeout;

            var response = RestClient.Execute(request);
            return response;
        }

        private static void AddUrlParametersToRequest(IRestRequest request, string urlParameters)
        {
            if (!string.IsNullOrEmpty(urlParameters))
                foreach (var param in urlParameters.Split('&').Select(parameter => parameter.Split('=')))
                    request.AddQueryParameter(param[0], param[1]);
        }

        private static void AddFormParametersToRequest(IRestRequest request, string formParameters)
        {
            if (!string.IsNullOrEmpty(formParameters))
                foreach (var param in formParameters.Split('&').Select(parameter => parameter.Split('=')))
                    request.AddParameter(param[0], param[1]);
        }

        private static HttpStatusCode ParseHttpStatusCode(string statusCode)
        {
            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCode);
        }
    }
}
