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

        public void Fake(Func<FluentFaker, FluentFaker> fake)
        {
            var fakeRequestResponseRepository = _container.Resolve<IFakeRequestResponseRepository>();
            var configuration = _container.Resolve<ITinyFakeHostConfiguration>();

            fake(new FluentFaker(fakeRequestResponseRepository, configuration));
        }
    }
}
