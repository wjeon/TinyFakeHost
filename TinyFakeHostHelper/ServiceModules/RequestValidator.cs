using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Primitives;
using TinyFakeHostHelper.Extensions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.ServiceModules
{
    public class RequestValidator : IRequestValidator
    {
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;

        public RequestValidator(IFakeRequestResponseRepository fakeRequestResponseRepository)
        {
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
        }

        public FakeHttpResponse GetValidatedFakeResponse(Method method, string url, IEnumerable<KeyValuePair<string, StringValues>> query, IEnumerable<KeyValuePair<string, StringValues>> form, string body)
        {
            const string defaultContentType = "application/json";

            var response = new FakeHttpResponse { ContentType = defaultContentType };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponseRepository.GetAll().OrderByDescending(f => f.Created))
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Method.Equals(method) && fakeRequest.Path.Matches(url) &&
                    fakeRequest.UrlParameters.Matches(query) && fakeRequest.FormParameters.Matches(form) &&
                    (fakeRequest.Body.Matches(body) || method.IsBodyAllowedMethod() && fakeRequest.FormParameters.Matches(body))
                ) {
                    var fakeResponse = fakeRequestResponse.FakeResponse;

                    if (fakeResponse.MillisecondsSleep > 0)
                        Thread.Sleep(fakeResponse.MillisecondsSleep);

                    response = fakeResponse.ToResponse(defaultContentType);
                    requestFound = true;
                    break;
                }
            }

            if (!requestFound) response.StatusCode = (int)HttpStatusCode.BadRequest;

            return response;
        }
    }
}