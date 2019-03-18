using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using RestSharp;
using TinyFakeHostHelper.Tests.Integration.Extensions;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    public class TinyFakeHostTests : TestBase
    {
        private IRestRequest _request;

        [SetUp]
        public void Given()
        {
            const string resourcePath = "/helloWorld";

            _request = CreateRequest(resourcePath, Method.GET);
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

        [Test]
        public void When_2_TinyFakeHosts_run_concurrently_with_the_same_port_number_one_waits_until_the_other_stops()
        {
            var runFakeHostThread = new Thread(() => RunFakeHostFor(3.Seconds()));

            runFakeHostThread.Start();

            Thread.Sleep(2.Seconds());

            Assert.DoesNotThrow(() => RunFakeHostFor(10.Milliseconds()));
        }

        private static void RunFakeHostFor(TimeSpan duration)
        {
            using (var tinyFakeHost = new TinyFakeHost(BaseUri))
            {
                tinyFakeHost.Start();

                Thread.Sleep(duration);

                tinyFakeHost.Stop();
            }
        }

        private void AssertThatWebServiceIsRunning()
        {
            var response = RestClient.Execute(_request);
            Assert.AreEqual(ResponseStatus.Completed, response.ResponseStatus);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private void AssertThatWebServiceIsNotRunning()
        {
            var response = RestClient.Execute(_request);
            Assert.AreEqual(ResponseStatus.Error, response.ResponseStatus);
            Assert.AreEqual(typeof (WebException), response.ErrorException.GetType());
            Assert.That(response.ErrorMessage.StartsWith("No connection could be made because the target machine actively refused it"));
        }
    }
}
