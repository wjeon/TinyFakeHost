using System.Collections.Generic;
using System.Linq;
using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.ServiceModules
{
    public class FakeServiceModule : NancyModule
    {
        private readonly IEnumerable<FakeRequestResponse> _fakeRequestResponses;

        public FakeServiceModule(IEnumerable<FakeRequestResponse> fakeRequestResponses)
        {
            _fakeRequestResponses = fakeRequestResponses;

            BuildRoutesForGetRequest();
        }

        private void BuildRoutesForGetRequest()
        {
            var segments = string.Empty;

            for (var i = 0; i < 10; i++)
            {
                segments += "/{segment" + i + "}";
                Get[segments] = p => ReturnFakeResult();
            }
        }

        private dynamic ReturnFakeResult()
        {
            var response = new Response { ContentType = "application/json" };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponses)
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Path.Equals(Request.Url.Path) && fakeRequest.Parameters.Equals(QueryParameters))
                {
                    var fakeResponse = fakeRequestResponse.FakeResponse;

                    response = fakeResponse.ToNancyResponse();
                    requestFound = true;
                    break;
                }
            }

            if (!requestFound) response.StatusCode = HttpStatusCode.BadRequest;

            return response;
        }

        private string QueryParameters
        {
            get
            {
                DynamicDictionary query = Request.Query;
                return string.Join("&", query.Keys.Select(key => string.Format("{0}={1}", key, query[key])));
            }
        }
    }
}
