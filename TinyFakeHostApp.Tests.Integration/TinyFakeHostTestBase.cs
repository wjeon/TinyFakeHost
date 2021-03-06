using NUnit.Framework;
using TinyFakeHostApp.Fakers;

namespace TinyFakeHostApp.Tests.Integration
{
    public abstract class TinyFakeHostTestBase : TestBase
    {
        protected TinyFakeHost TinyFakeHost;
        protected RequestResponseFaker Faker;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            BaseUri = BaseUriWithRandomPortNumber();

            TinyFakeHost = new TinyFakeHost(BaseUri);

            TinyFakeHost.Start();

            Faker = TinyFakeHost.GetFaker();
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            TinyFakeHost.Stop();

            TinyFakeHost.Dispose();
        }
    }
}