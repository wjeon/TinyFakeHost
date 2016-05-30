using System;

namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeRequestResponse
    {
        public FakeRequest FakeRequest { get; set; }
        public FakeResponse FakeResponse { get; set; }
        public DateTimeOffset Created { get; set; }

        public FakeRequestResponse()
        {
            FakeRequest = new FakeRequest();
            FakeResponse = new FakeResponse();
        }
    }
}
