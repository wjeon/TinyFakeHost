using System;
using NUnit.Framework;

namespace TinyFakeHostApp.Tests.Unit
{
    [TestFixture]
    public class TinyFakeHostTests
    {
        private const string Uri = "http://localhost:5432/someService/v1/";

        [Test]
        public void When_construct_TinyFakeHost_it_is_constructed_with_host_uri()
        {
            Assert.DoesNotThrow(() => new TinyFakeHost(Uri));
        }

        [Test]
        public void When_construct_TinyFakeHost_it_implements_IDisposable()
        {
            var tinyFakeHost = new TinyFakeHost(Uri);

            Assert.IsInstanceOf(typeof(IDisposable), tinyFakeHost);
        }
    }
}
