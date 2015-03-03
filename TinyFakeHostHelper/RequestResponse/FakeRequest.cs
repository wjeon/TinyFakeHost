using System.Collections.Generic;

namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeRequest
    {
        public FakeRequest()
        {
            Headers = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Headers { get; set; }
        public string Path { get; set; }
        public string Parameters { get; set; }
    }
}