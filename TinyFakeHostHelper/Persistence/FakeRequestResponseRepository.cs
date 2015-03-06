using System.Collections.Generic;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Persistence
{
    public class FakeRequestResponseRepository : IFakeRequestResponseRepository
    {
        private readonly IList<FakeRequestResponse> _fakeRequestRequestResponses;

        public FakeRequestResponseRepository()
        {
            _fakeRequestRequestResponses = new List<FakeRequestResponse>();
        }

        public void Add(FakeRequestResponse fakeRequestResponse)
        {
            _fakeRequestRequestResponses.Add(fakeRequestResponse);
        }

        public IEnumerable<FakeRequestResponse> GetAll()
        {
            return _fakeRequestRequestResponses;
        }
    }
}
