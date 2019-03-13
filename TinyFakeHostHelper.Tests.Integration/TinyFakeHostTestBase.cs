using System;
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
            var port = new Random().Next(60000, 60020);

            BaseUri = string.Format("http://localhost:{0}/", port);

            Console.WriteLine(BaseUri);

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