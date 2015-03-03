using System.Net;
using NUnit.Framework;
using RestSharp;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    public class TinyFakeHostTests : TinyFakeHostTestBase
    {
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
            var response = RestClient.Execute(Request);
            Assert.AreEqual(ResponseStatus.Completed, response.ResponseStatus);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private void AssertThatWebServiceIsNotRunning()
        {
            var response = RestClient.Execute(Request);
            Assert.AreEqual(ResponseStatus.Error, response.ResponseStatus);
            Assert.AreEqual(typeof (WebException), response.ErrorException.GetType());
            Assert.AreEqual("Unable to connect to the remote server", response.ErrorMessage);
        }
    }
}
