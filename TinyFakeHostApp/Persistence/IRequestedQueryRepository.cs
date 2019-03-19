using System.Collections.Generic;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.Persistence
{
    public interface IRequestedQueryRepository
    {
        void Add(FakeRequest requestedQuery);
        IEnumerable<FakeRequest> GetAll();
    }
}