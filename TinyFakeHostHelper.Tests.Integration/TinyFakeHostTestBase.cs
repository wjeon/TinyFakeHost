using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    public abstract class TinyFakeHostTestBase
    {
        protected const string BaseUri = "http://localhost:5432/";
        protected IRestClient RestClient;

        [SetUp]
        public void SetUp()
        {
            RestClient = new RestClient(BaseUri);
        }

        protected IRestRequest CreateRequest(string resourcePath)
        {
            return new RestRequest(resourcePath, Method.GET);
        }
    }
}