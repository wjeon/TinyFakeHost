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

        /// <summary>
        /// Assert requested query with the resouce path and optional parameters.
        /// </summary>
        /// <param name="fluentAssertion">
        /// Fluent requested query assertion details. Example: Assert( a => a.Resource("/products").WithParameters("type=desk").WasRequested() )
        /// </param>
        public void Assert(Func<FluentAsserter, FluentAsserter> fluentAssertion)
        {
            var requestedQueryRepository = _container.Resolve<IRequestedQueryRepository>();

            fluentAssertion(new FluentAsserter(requestedQueryRepository));
        }
    }
}