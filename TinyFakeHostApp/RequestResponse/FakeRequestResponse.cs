using System;

namespace TinyFakeHostApp.RequestResponse
{
    public class FakeRequestResponse
    {
        public Guid Id { get; private set; }
        public FakeRequest FakeRequest { get; set; }
        public FakeResponse FakeResponse { get; set; }
        public DateTimeOffset Created { get; set; }

        public FakeRequestResponse()
        {
            Id = Guid.NewGuid();
            FakeRequest = new FakeRequest();
            FakeResponse = new FakeResponse();
        }
    }
}
