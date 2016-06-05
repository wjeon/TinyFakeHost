using System;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.Fakers
{
    public class RequestResponseFaker
    {
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;
        private readonly FluentFaker _fluentFaker;

        public RequestResponseFaker(IFakeRequestResponseRepository fakeRequestResponseRepository, ITinyFakeHostConfiguration configuration)
        {
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
            _fluentFaker = new FluentFaker(_fakeRequestResponseRepository, configuration);
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

        public void DeleteAllFakes()
        {
            _fakeRequestResponseRepository.DeleteAll();
            _fluentFaker.LastCreatedFakeId = null;
        }
    }
}
