using System.Collections.Generic;
using TinyFakeHostHelper.RequestResponse;
using TinyFakeHostHelper.Supports;

namespace TinyFakeHostHelper.Persistence
{
    public class FakeRequestResponseRepository : IFakeRequestResponseRepository
    {
        private readonly IList<FakeRequestResponse> _fakeRequestRequestResponses;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FakeRequestResponseRepository(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _fakeRequestRequestResponses = new List<FakeRequestResponse>();
        }

        public void Add(FakeRequestResponse fakeRequestResponse)
        {
            fakeRequestResponse.Created = _dateTimeProvider.UtcNow;
            _fakeRequestRequestResponses.Add(fakeRequestResponse);
        }

        public IEnumerable<FakeRequestResponse> GetAll()
        {
            return _fakeRequestRequestResponses;
        }
    }
}
