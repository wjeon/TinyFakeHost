using System;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.Fakers
{
    public class RequestResponseFaker
    {
        private readonly FluentFaker _fluentFaker;

        public RequestResponseFaker(TinyIoCContainer container)
        {
            var fakeRequestResponseRepository = container.Resolve<IFakeRequestResponseRepository>();
            var configuration = container.Resolve<ITinyFakeHostConfiguration>();
            _fluentFaker = new FluentFaker(fakeRequestResponseRepository, configuration);
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
    }
}
