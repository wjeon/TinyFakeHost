using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.ServiceModules;
using TinyFakeHostHelper.Supports;

namespace TinyFakeHostHelper
{
    public class TinyFakeHostBootstrapper : DefaultNancyBootstrapper
    {
        private TinyIoCContainer _container;
        private readonly ITinyFakeHostConfiguration _fakeHostConfiguration;

        public TinyFakeHostBootstrapper(ITinyFakeHostConfiguration fakeHostConfiguration)
        {
            _fakeHostConfiguration = fakeHostConfiguration;
        }

        public TinyIoCContainer GetTinyIoCContainer()
        {
            return _container;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register(_fakeHostConfiguration);
            container.Register<IDateTimeProvider, DateTimeProvider>();
            container.Register<IFakeRequestResponseRepository, FakeRequestResponseRepository>();
            container.Register<IRequestedQueryRepository, RequestedQueryRepository>();
            container.Register<IRequestValidator, RequestValidator>();
            _container = container;
        }
    }
}
