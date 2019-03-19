using System;
using System.Collections.Generic;
using System.Linq;
using TinyFakeHostApp.Exceptions;
using TinyFakeHostApp.RequestResponse;
using TinyFakeHostApp.Supports;

namespace TinyFakeHostApp.Persistence
{
    public class FakeRequestResponseRepository : IFakeRequestResponseRepository
    {
        private IList<FakeRequestResponse> _fakeRequestResponses;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FakeRequestResponseRepository(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
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

            fakeRequestResponse.Created = _dateTimeProvider.UtcNow;

            _fakeRequestResponses.Add(fakeRequestResponse);
        }

        public IEnumerable<FakeRequestResponse> GetAll()
        {
            return _fakeRequestResponses;
        }

        public void DeleteById(Guid id)
        {
            var fakeRequestResponse = _fakeRequestResponses.ToList().Find(f => f.Id == id);

            if (fakeRequestResponse != null)
                _fakeRequestResponses.Remove(fakeRequestResponse);
        }

        public void DeleteAll()
        {
            _fakeRequestResponses = new List<FakeRequestResponse>();
        }
    }
}
