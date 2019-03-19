using System;
using System.Collections.Generic;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.Persistence
{
    public interface IFakeRequestResponseRepository
    {
        void Add(FakeRequestResponse fakeRequestResponse);
        IEnumerable<FakeRequestResponse> GetAll();
        void DeleteById(Guid id);
        void DeleteAll();
    }
}