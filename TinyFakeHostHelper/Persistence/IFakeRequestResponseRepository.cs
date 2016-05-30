using System;
using System.Collections.Generic;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Persistence
{
    public interface IFakeRequestResponseRepository
    {
        void Add(FakeRequestResponse fakeRequestResponse);
        IEnumerable<FakeRequestResponse> GetAll();
        void DeleteById(Guid id);
        void DeleteAll();
    }
}