using System.Linq;
using NUnit.Framework;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.Supports;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class RequestResponseFakerTests
    {
        private IFakeRequestResponseRepository _repository;
        private RequestResponseFaker _faker;

        [SetUp]
        public void SetUp()
        {
            _repository = new FakeRequestResponseRepository(new DateTimeProvider());
            _faker = new RequestResponseFaker(_repository, new TinyFakeHostConfiguration());
        }

        [Test]
        public void LastCreatedFakeId_property_returns_id_of_last_created_fake()
        {
            _faker.Fake(f => f.IfRequest("/pathForFirstFake").ThenReturn(new FakeResponse()));
            _faker.Fake(f => f.IfRequest("/pathForSecondFake").ThenReturn(new FakeResponse()));

            var lastCreatedFakeId = _repository.GetAll().ToList().Find(a => a.FakeRequest.Path == "/pathForSecondFake").Id;

            Assert.AreEqual(_faker.LastCreatedFakeId, lastCreatedFakeId);
        }
    }
}
