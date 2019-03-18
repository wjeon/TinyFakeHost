using System.Collections.Generic;
using System.Net;
using TinyFakeHostHelper.ServiceModules;

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
        public HttpStatusCode StatusCode { get; set; }
        public int MillisecondsSleep { get; set; }

        public FakeHttpResponse ToResponse(string defaultContentType)
        {
            return new FakeHttpResponse
            {
                Body = Content,
                ContentType = ContentType ?? defaultContentType,
                Headers = Headers,
                StatusCode = (int)StatusCode
            };
        }
    }
}