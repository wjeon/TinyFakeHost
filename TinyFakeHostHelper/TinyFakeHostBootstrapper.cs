using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper
{
    public class TinyFakeHostBootstrapper : DefaultNancyBootstrapper
    {
        private TinyIoCContainer _container;

        public TinyIoCContainer GetTinyIoCContainer()
        {
            return _container;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<IFakeRequestResponseRepository, FakeRequestResponseRepository>();
            _container = container;
        }
    }
}
