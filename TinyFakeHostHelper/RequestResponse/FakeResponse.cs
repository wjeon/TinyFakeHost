using System.Collections.Generic;

namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeResponse
    {
        public FakeResponse()
        {
            Headers = new Dictionary<string, string>();
            StatusCode = System.Net.HttpStatusCode.OK;
        }

        public string ContentType { get; set; }
        public string Content { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string ReasonPhrase { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }
}