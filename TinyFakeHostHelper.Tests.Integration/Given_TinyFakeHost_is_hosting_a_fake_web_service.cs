using NUnit.Framework;

namespace TinyFakeHostHelper.Tests.Integration
{
    [TestFixture]
    class Given_TinyFakeHost_is_hosting_a_fake_web_service : TinyFakeHostTestBase
    {
        private TinyFakeHost _tinyFakeHost;

        [SetUp]
        public void Given()
        {
            _tinyFakeHost = new TinyFakeHost(BaseUri);

            _tinyFakeHost.Start();
        }

        [TestCase("/helloWorld", "Hello world")]
        [TestCase("/vendors/9876-5432-1098-7654/products", @"[{""id"":460173,""name"":""Product A"",""type"":""chair"",""manufactureYear"":2014},{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]")]
        public void When_a_web_client_queries_the_fake_web_service_it_returns_a_fake_content(string resourcePath, string responseContent)
        {
            var request = CreateRequest(resourcePath);

            var response = RestClient.Execute(request);

            Assert.AreEqual(responseContent, response.Content);
        }

        [TearDown]
        public void TearDown()
        {
            _tinyFakeHost.Stop();

            _tinyFakeHost.Dispose();
        }
    }
}
