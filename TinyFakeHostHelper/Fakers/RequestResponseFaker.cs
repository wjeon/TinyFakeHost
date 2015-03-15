using System;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.Fakers
{
    public class RequestResponseFaker
    {
        private readonly TinyIoCContainer _container;

        public RequestResponseFaker(TinyIoCContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Fake web response of the query request with the resouce path and optional parameters.
        /// </summary>
        /// <param name="fluentFake">
        /// Fluent fake request response details. Example: Fake( f => f.IfRequest("/products").WithParameters("type=desk").ThenReturn(fakeResponse) )
        /// </param>
        public void Fake(Func<FluentFaker, FluentFaker> fluentFake)
        {
            var fakeRequestResponseRepository = _container.Resolve<IFakeRequestResponseRepository>();
            var configuration = _container.Resolve<ITinyFakeHostConfiguration>();

            fluentFake(new FluentFaker(fakeRequestResponseRepository, configuration));
        }
    }
}
