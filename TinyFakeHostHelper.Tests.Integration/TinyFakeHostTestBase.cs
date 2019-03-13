using System;
using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    public abstract class TinyFakeHostTestBase
    {
        protected static string BaseUri;
        protected IRestClient RestClient;

        [SetUp]
        public void SetUp()
        {
            var port = new Random().Next(60000, 60020);

            BaseUri = string.Format("http://localhost:{0}/", port);

            Console.WriteLine(BaseUri);

            RestClient = new RestClient(BaseUri);
        }

        protected IRestRequest CreateRequest(string resourcePath, Method method)
        {
            return new RestRequest(resourcePath, method);
        }
    }
}