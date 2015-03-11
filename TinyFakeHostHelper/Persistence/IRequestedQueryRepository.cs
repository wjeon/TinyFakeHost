using System.Collections.Generic;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Persistence
{
    public interface IRequestedQueryRepository
    {
        void Add(FakeRequest requestedQuery);
        IEnumerable<FakeRequest> GetAll();
    }
}