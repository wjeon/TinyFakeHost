using System.Collections.Generic;

namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeRequest
    {
        public FakeRequest()
        {
            Headers = new Dictionary<string, string>();
            Parameters = new UrlParameters();
        }

        public IDictionary<string, string> Headers { get; set; }
        public string Path { get; set; }
        public UrlParameters Parameters { get; set; }
    }
}