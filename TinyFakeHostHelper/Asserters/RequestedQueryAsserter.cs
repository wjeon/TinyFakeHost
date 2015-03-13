using System;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.Asserters
{
    public class RequestedQueryAsserter
    {
        private readonly TinyIoCContainer _container;

        public RequestedQueryAsserter(TinyIoCContainer container)
        {
            _container = container;
        }

        public void Assert(Func<FluentAsserter, FluentAsserter> asserter)
        {
            var requestedQueryRepository = _container.Resolve<IRequestedQueryRepository>();

            asserter(new FluentAsserter(requestedQueryRepository));
        }
    }
}