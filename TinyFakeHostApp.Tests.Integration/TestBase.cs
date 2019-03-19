using System;
using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostApp.Tests.Integration
{
    public abstract class TestBase
    {
        protected static string BaseUri;
        protected IRestClient RestClient;

        [SetUp]
        public void SetUp()
        {
            BaseUri = BaseUri ?? BaseUriWithRandomPortNumber();

            RestClient = new RestClient(BaseUri);
        }

        protected string BaseUriWithRandomPortNumber()
        {
            var port = new Random().Next(60000, 60020);

            var baseUri = string.Format("http://localhost:{0}/", port);

            Console.WriteLine(BaseUri);

            return baseUri;
        }

        protected IRestRequest CreateRequest(string resourcePath, Method method)
        {
            return new RestRequest(resourcePath, method);
        }
    }
}