using NUnit.Framework;
using TinyFakeHostHelper.Fakers;

namespace TinyFakeHostHelper.Tests.Integration
{
    public abstract class TinyFakeHostTestBase : TestBase
    {
        protected TinyFakeHost TinyFakeHost;
        protected RequestResponseFaker Faker;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            TinyFakeHost = new TinyFakeHost(BaseUri);

            TinyFakeHost.Start();

            Faker = TinyFakeHost.GetFaker();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            TinyFakeHost.Stop();

            TinyFakeHost.Dispose();
        }
    }
}