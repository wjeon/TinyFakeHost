using System.Collections.Generic;

namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeHttpResponse
    {
        public string ContentType { get; set; }
        public string Body { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public int StatusCode { get; set; }
    }
}