using System;
using Nancy.TinyIoc;

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
            fake(new FluentFaker(_container));
        }
    }
}
