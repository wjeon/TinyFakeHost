using System.Collections.Generic;
using System.Linq;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Persistence
{
    public class FakeRequestResponseRepository : IFakeRequestResponseRepository
    {
        private readonly IList<FakeRequestResponse> _fakeRequestResponses;

        public FakeRequestResponseRepository()
        {
            _fakeRequestResponses = new List<FakeRequestResponse>();
        }

        public void Add(FakeRequestResponse fakeRequestResponse)
        {
            if (_fakeRequestResponses.Any(f => f.Id == fakeRequestResponse.Id))
                throw new UniqueIdException(
                    string.Format(
                        "Id of FakeRequestResponse '{0}' already exists in stored fakes",
                        fakeRequestResponse.Id
                    )
                );

            _fakeRequestResponses.Add(fakeRequestResponse);
        }

        public IEnumerable<FakeRequestResponse> GetAll()
        {
            return _fakeRequestResponses;
        }
    }
}
