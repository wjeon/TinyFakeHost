using System.Collections.Generic;
using Nancy;

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

        public Response ToNancyResponse()
        {
            Response response = Content;
            response.ContentType = ContentType;
            response.Headers = Headers;
            response.StatusCode = (HttpStatusCode)StatusCode;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}