using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    public abstract class TestBase
    {
        protected static string BaseUri;
        protected IRestClient RestClient;

        [SetUp]
        public void SetUp()
        {
            RestClient = new RestClient(BaseUri);
        }

        protected IRestRequest CreateRequest(string resourcePath, Method method)
        {
            return new RestRequest(resourcePath, method);
        }
    }
}