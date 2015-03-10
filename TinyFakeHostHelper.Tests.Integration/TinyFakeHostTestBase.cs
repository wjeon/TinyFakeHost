using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    public abstract class TinyFakeHostTestBase
    {
        protected const string BaseUri = "http://localhost:5432/";
        protected const string ResourcePath = "/helloWorld";
        protected IRestClient RestClient;
        protected IRestRequest Request;

        [SetUp]
        public void SetUp()
        {
            RestClient = new RestClient(BaseUri);

            Request = CreateRequest(ResourcePath);
        }

        protected IRestRequest CreateRequest(string resourcePath)
        {
            return new RestRequest(resourcePath, Method.GET);
        }
    }
}