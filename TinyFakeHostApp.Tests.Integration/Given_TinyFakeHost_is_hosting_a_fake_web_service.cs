using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using RestSharp;
using TinyFakeHostApp.RequestResponse;
using AssertionException = TinyFakeHostApp.Exceptions.AssertionException;
using Method = RestSharp.Method;

namespace TinyFakeHostApp.Tests.Integration
{
    [TestFixture]
    class Given_TinyFakeHost_is_hosting_a_fake_web_service : TinyFakeHostTestBase
    {
        private const string ResourcePath = "/resourcePath";
        private const string UrlParameter = "param=value";

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

            Faker.Fake(f => f
                .IfRequest(resourcePath)
                .WithMethod(TinyFakeHostMethod(requestMethod))
                .WithUrlParameters(urlParameters)
                .WithFormParameters(formParameters)
                .ThenReturn(new FakeResponse { ContentType = "application/json", Content = responseContent, StatusCode = ParseHttpStatusCode("OK") })
            );

            var request = new RestRequest(resourcePath, RestSharpMethod(requestMethod));

            AddUrlParametersToRequest(request, urlParameters);
            AddFormParametersToRequest(request, formParameters);

            TinyFakeHost.RequestedQueryPrint = true;

            var response = RestClient.Execute(request);

            Assert.AreEqual(responseContent, response.Content);
        }

        [Test]
        public void When_a_web_client_queries_the_fake_web_service_with_body_it_returns_a_fake_content_correctly()
        {
            const string responseContent = @"{""messageId"":389317,""messageTitle"":""Product B"",""dateCreated"":""2018-07-22T19:26:35-07:00""}";
            const string resourcePath = "/vendors/9876-5432-1098-7654/questions";
            const string body = @"{""title"":""Question"",""message"":""How much is the item number 1234""}";

            Faker.Fake(f => f
                .IfRequest(resourcePath)
                .WithMethod(TinyFakeHostApp.RequestResponse.Method.POST)
                .WithBody(body)
                .ThenReturn(new FakeResponse { ContentType = "application/json", Content = responseContent, StatusCode = HttpStatusCode.Created })
            );

            var request = new RestRequest(resourcePath, Method.POST);

            request.AddParameter(
                new RestSharp.Parameter { Type = ParameterType.RequestBody, Name = "application/json", Value = body });

            TinyFakeHost.RequestedQueryPrint = true;

            var response = RestClient.Execute(request);

            Assert.AreEqual(responseContent, response.Content);
        }

        private static TinyFakeHostApp.RequestResponse.Method TinyFakeHostMethod(string method)
        {
            return (TinyFakeHostApp.RequestResponse.Method)Enum.Parse(typeof(TinyFakeHostApp.RequestResponse.Method), method);
        }

        private static Method RestSharpMethod(string method)
        {
            return (Method)Enum.Parse(typeof(Method), method);
        }

        [TestCase("/vendors/9876-5432-1098-7654/products", "type=desk&manufactureYear=2013", @"[{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]", "application/json", "OK")]
        [TestCase("/invalidPath", "param=invalidParameter", @"{""message"":""Bad Request""}", "application/json", "BadRequest")]
        public void When_a_web_client_queries_the_fake_web_service_with_parameters_it_returns_a_fake_content(string resourcePath, string urlParameters, string responseContent, string contentType, string statusCode)
        {
            Faker.Fake(f => f
                .IfRequest(resourcePath)
                .WithUrlParameters(urlParameters)
                .ThenReturn(new FakeResponse { ContentType = contentType, Content = responseContent, StatusCode = ParseHttpStatusCode(statusCode) })
            );

            TinyFakeHost.RequestedQueryPrint = true;

            var response = CallFakeService(resourcePath, urlParameters);

            Assert.AreEqual(responseContent, response.Content);
        }

        [TestCase("/", @"{""message"":""This is the root of the site""]", "application/json", "OK")]
        [TestCase("/helloWorld", "Hello world", "text/plain", "BadRequest")]
        [TestCase("/vendors/9876-5432-1098-7654/products", @"[{""id"":460173,""name"":""Product A"",""type"":""chair"",""manufactureYear"":2014},{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]", "application/json", "OK")]
        public void When_a_web_client_queries_the_fake_web_service_without_parameters_it_returns_a_fake_content(string resourcePath, string responseContent, string contentType, string statusCode)
        {
            Faker.Fake(f => f
                .IfRequest(resourcePath)
                .ThenReturn(new FakeResponse { ContentType = contentType, Content = responseContent, StatusCode = ParseHttpStatusCode(statusCode) })
            );

            TinyFakeHost.RequestedQueryPrint = true;

            var response = CallFakeService(resourcePath);

            Assert.AreEqual(responseContent, response.Content);
        }

        [Test]
        public void When_a_web_client_queries_the_fake_web_service_with_long_sleep_time_it_fakes_long_process_time()
        {
            const string resourcePath = "/timeout";

            Faker.Fake(f => f
                .IfRequest(resourcePath)
                .ThenReturn(new FakeResponse{
                    ContentType = "application/json",
                    Content = @"{""message"":""Request Timeout""}",
                    StatusCode = HttpStatusCode.RequestTimeout,
                    MillisecondsSleep = 6000
                })
            );

            var response = CallFakeService(resourcePath, 3000);

            Assert.IsTrue(response.ErrorMessage.Contains("The operation has timed out"));
        }

        [Test]
        public void When_a_web_client_queries_the_fake_web_service_the_requested_query_is_stored()
        {
            CallFakeService(ResourcePath, UrlParameter);

            var requestedQueries = TinyFakeHost.GetRequestedQueries();

            Assert.IsTrue(requestedQueries.Any(a => a.Path == ResourcePath && a.UrlParameters.ToString() == UrlParameter));
        }

        [Test]
        public void When_fake_host_asserts_requested_query_correctly_with_resource_path_and_parameter_it_does_not_throw_exception()
        {
            CallFakeService(ResourcePath, UrlParameter);

            var asserter = TinyFakeHost.GetAsserter();

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

            var asserter = TinyFakeHost.GetAsserter();

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

            var asserter = TinyFakeHost.GetAsserter();

            Assert.DoesNotThrow(() =>
                asserter.Assert(a => a
                    .Resource(ResourcePath)
                    .WithMethod(TinyFakeHostApp.RequestResponse.Method.PUT)
                    .WithFormParameters("param=value")
                    .WasRequested()
                )
            );
        }

        [Test]
        public void When_fake_host_asserts_requested_query_incorrectly_with_wrong_resource_path_it_throws_assertion_exception()
        {
            CallFakeService(ResourcePath);

            var asserter = TinyFakeHost.GetAsserter();

            Assert.Throws<AssertionException>(() =>
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

            var asserter = TinyFakeHost.GetAsserter();

            Assert.Throws<AssertionException>(() =>
                asserter.Assert(a => a
                    .Resource(ResourcePath)
                    .WithUrlParameters("param=wrong+parameter")
                    .WasRequested()
                )
            );
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
