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

        public override string ToString()
        {
            return string.Format("Resource Path: {0}, Parameters: {1}", Path, Parameters);
        }
    }
}