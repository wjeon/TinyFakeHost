using System.Collections.Generic;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.Persistence
{
    public class RequestedQueryRepository : IRequestedQueryRepository
    {
        private readonly IList<FakeRequest> _requestedQueries;

        public RequestedQueryRepository()
        {
            _requestedQueries = new List<FakeRequest>();
        }

        public void Add(FakeRequest requestedQuery)
        {
            _requestedQueries.Add(requestedQuery);
        }

        public IEnumerable<FakeRequest> GetAll()
        {
            return _requestedQueries;
        }
    }
}