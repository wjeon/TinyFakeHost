using System;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.Fakers
{
    public class RequestResponseFaker
    {
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;
        private readonly FluentFaker _fluentFaker;

        public RequestResponseFaker(IFakeRequestResponseRepository fakeRequestResponseRepository)
        {
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
            _fluentFaker = new FluentFaker(_fakeRequestResponseRepository);
        }

        /// <summary>
        /// Fake web response of the query request with the resouce path and optional parameters.
        /// </summary>
        /// <param name="fluentFake">
        /// Fluent fake request response details. Example: Fake( f => f.IfRequest("/products").WithParameters("type=desk").ThenReturn(fakeResponse) )
        /// </param>
        public void Fake(Func<FluentFaker, FluentFaker> fluentFake)
        {
            fluentFake(_fluentFaker);
        }

        public Guid? LastCreatedFakeId
        {
            get { return _fluentFaker.LastCreatedFakeId; }
        }

        public void DeleteFakeById(Guid id)
        {
            if (_fluentFaker.LastCreatedFakeId == id)
                _fluentFaker.LastCreatedFakeId = null;

            _fakeRequestResponseRepository.DeleteById(id);
        }

        public void DeleteLastCreatedFake()
        {
            if (!_fluentFaker.LastCreatedFakeId.HasValue)
            {
                Console.WriteLine("Cannot not find last created fake");
                return;
            }

            _fakeRequestResponseRepository.DeleteById(_fluentFaker.LastCreatedFakeId.Value);
            _fluentFaker.LastCreatedFakeId = null;
        }

        public void DeleteAllFakes()
        {
            _fakeRequestResponseRepository.DeleteAll();
            _fluentFaker.LastCreatedFakeId = null;
        }
    }
}
