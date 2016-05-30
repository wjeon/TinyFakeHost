using System;

namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeRequestResponse
    {
        public Guid Id { get; private set; }
        public FakeRequest FakeRequest { get; set; }
        public FakeResponse FakeResponse { get; set; }

        public FakeRequestResponse()
        {
            Id = Guid.NewGuid();
            FakeRequest = new FakeRequest();
            FakeResponse = new FakeResponse();
        }
    }
}
