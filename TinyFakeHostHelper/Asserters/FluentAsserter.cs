using System.Collections.Generic;
using System.Linq;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.Extensions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Asserters
{
    public class FluentAsserter
    {
        private readonly IRequestedQueryRepository _requestedQueryRepository;
        private FakeRequest _request;

        public FluentAsserter(IRequestedQueryRepository requestedQueryRepository)
        {
            _requestedQueryRepository = requestedQueryRepository;
        }

        public FluentAsserter Resource(string path)
        {
            _request = new FakeRequest { Path = path };

            return this;
        }

        public FluentAsserter WithParameters(string urlParameterString)
        {
            var parameters = urlParameterString.ParseParameters();

            return WithParameters(parameters);
        }

        public FluentAsserter WithParameters(IEnumerable<Parameter> urlParameters)
        {
            _request.UrlParameters = new Parameters(urlParameters);

            return this;
        }

        public FluentAsserter WasRequested()
        {
            AssertQueryWasRequested();

            ClearRequest();

            return this;
        }

        private void AssertQueryWasRequested()
        {
            var requestedQueries = _requestedQueryRepository.GetAll();

            if (!requestedQueries.Any(q =>
                q.Path.Equals(_request.Path) &&
                q.UrlParameters.OrderBy(r => r.Key).SequenceEqual(_request.UrlParameters.OrderBy(r => r.Key)))
            )
                throw new AssertionException(string.Format("The query with {0} was not requested", _request));
        }

        private void ClearRequest()
        {
            _request = null;
        }
    }
}