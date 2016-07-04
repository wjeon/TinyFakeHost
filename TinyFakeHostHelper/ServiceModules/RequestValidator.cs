﻿using System.Linq;
using System.Threading;
using Nancy;
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

        public Response GetValidatedFakeResponse(Method method, Url url, DynamicDictionary query, DynamicDictionary form, string body)
        {
            var response = new Response { ContentType = "application/json" };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponseRepository.GetAll().OrderByDescending(f => f.Created))
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Method.Equals(method) && fakeRequest.Path.Matches(url.Path) &&
                    fakeRequest.UrlParameters.Matches(query) && fakeRequest.FormParameters.Matches(form) &&
                    (fakeRequest.Body.Matches(body) || (method.IsBodyAllowedMethod() && fakeRequest.FormParameters.Matches(body)))
                ) {
                    var fakeResponse = fakeRequestResponse.FakeResponse;

                    if (fakeResponse.MillisecondsSleep > 0)
                        Thread.Sleep(fakeResponse.MillisecondsSleep);

                    response = fakeResponse.ToNancyResponse();
                    requestFound = true;
                    break;
                }
            }

            if (!requestFound) response.StatusCode = HttpStatusCode.BadRequest;

            return response;
        }
    }
}