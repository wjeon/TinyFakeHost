using NUnit.Framework;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class TinyFakeHostTests
    {
        [Test]
        public void When_construct_TinyFakeHost_it_is_constructed_with_host_uri()
        {
            const string uri = "http://localhost:5432/someService/v1/";
            Assert.DoesNotThrow(() => new TinyFakeHost(uri));
        }
    }
}
