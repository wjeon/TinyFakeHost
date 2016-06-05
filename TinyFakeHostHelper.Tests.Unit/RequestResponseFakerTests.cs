using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
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

        [Test]
        public void DeleteFakeById_method_calls_DeleteById_method_in_FakeRequestResponseRepository()
        {
            var id = Guid.NewGuid();
            var repository = MockRepository.GenerateStub<IFakeRequestResponseRepository>();
            _faker = new RequestResponseFaker(repository, new TinyFakeHostConfiguration());

            _faker.DeleteFakeById(id);

            repository.AssertWasCalled(r => r.DeleteById(id));
        }

        [Test]
        public void When_delete_fake_by_last_created_fake_id_it_also_sets_the_stored_last_created_fake_id_to_null()
        {
            _faker.Fake(f => f.IfRequest("/pathForLastFake").ThenReturn(new FakeResponse()));
            Assert.IsNotNull(_faker.LastCreatedFakeId);
            var lastCreatedFakeId = _faker.LastCreatedFakeId;

            _faker.DeleteFakeById(lastCreatedFakeId.Value);

            Assert.IsNull(_faker.LastCreatedFakeId);
        }

        [Test]
        public void DeleteLastCreatedFake_method_deletes_fake_by_stored_last_created_fake_id()
        {
            _faker.Fake(f => f.IfRequest("/pathForLastFake").ThenReturn(new FakeResponse()));
            Assert.IsNotNull(StoredLastFake());

            _faker.DeleteLastCreatedFake();

            Assert.IsNull(StoredLastFake());
        }

        private FakeRequestResponse StoredLastFake()
        {
            return _repository.GetAll().ToList().Find(a => a.FakeRequest.Path == "/pathForLastFake");
        }
    }
}
