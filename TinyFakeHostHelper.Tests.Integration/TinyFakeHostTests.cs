using System.Net;
using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    public class TinyFakeHostTests
    {
        private const string BaseUri = "http://localhost:5432/";
        private const string RequestPath = "/";
        private IRestClient _restClient;
        private IRestRequest _request;

        [SetUp]
        public void SetUp()
        {
            _restClient = new RestClient(BaseUri);

            _request = new RestRequest(RequestPath, Method.GET);
        }

        [Test]
        public void When_start_and_stop_TinyFakeHost_it_starts_and_stops_a_fake_service()
        {
            using (var tinyFakeHost = new TinyFakeHost(BaseUri))
            {
                AssertThatWebServiceIsNotRunning();

                tinyFakeHost.Start();

                AssertThatWebServiceIsRunning();

                tinyFakeHost.Stop();

                AssertThatWebServiceIsNotRunning();
            }
        }

        private void AssertThatWebServiceIsRunning()
        {
            var response = _restClient.Execute(_request);
            Assert.AreEqual(ResponseStatus.Completed, response.ResponseStatus);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private void AssertThatWebServiceIsNotRunning()
        {
            var response = _restClient.Execute(_request);
            Assert.AreEqual(ResponseStatus.Error, response.ResponseStatus);
            Assert.AreEqual(typeof (WebException), response.ErrorException.GetType());
            Assert.AreEqual("Unable to connect to the remote server", response.ErrorMessage);
        }
    }
}
