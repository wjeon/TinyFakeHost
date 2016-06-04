using System.Linq;
using NUnit.Framework;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class RequestResponseFakerTests
    {
        [Test]
        public void LastCreatedFakeId_property_returns_id_of_last_created_fake()
        {
            var repository = new FakeRequestResponseRepository();
            var faker = new RequestResponseFaker(repository, new TinyFakeHostConfiguration());

            faker.Fake(f => f.IfRequest("/pathForFirstFake").ThenReturn(new FakeResponse()));
            faker.Fake(f => f.IfRequest("/pathForSecondFake").ThenReturn(new FakeResponse()));

            var lastCreatedFakeId = repository.GetAll().ToList().Find(a => a.FakeRequest.Path == "/pathForSecondFake").Id;

            Assert.AreEqual(faker.LastCreatedFakeId, lastCreatedFakeId);
        }
    }
}
