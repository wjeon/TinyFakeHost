﻿using System.Linq;
using System.Threading;
using Nancy;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.ServiceModules
{
    public class FakeServiceModule : NancyModule
    {
        private readonly ITinyFakeHostConfiguration _tinyFakeHostConfiguration;
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;
        private readonly IRequestedQueryRepository _requestedQueryRepository;

        public FakeServiceModule(ITinyFakeHostConfiguration tinyFakeHostConfiguration, IFakeRequestResponseRepository fakeRequestResponseRepository, IRequestedQueryRepository requestedQueryRepository)
        {
            _tinyFakeHostConfiguration = tinyFakeHostConfiguration;
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
            _requestedQueryRepository = requestedQueryRepository;

            BuildRoutesForGetRequest();
        }

        private void BuildRoutesForGetRequest()
        {
            var segments = string.Empty;

            for (var i = 0; i < _tinyFakeHostConfiguration.MaximumNumberOfUrlPathSegments; i++)
            {
                segments += "/{segment" + i + "}";
                Get[segments] = p => ReturnFakeResult();
            }
        }

        private dynamic ReturnFakeResult()
        {
            var query = Request.Query as DynamicDictionary;

            var requestedQuery = new FakeRequest
            {
                Path = Request.Url.Path,
                Parameters = new UrlParameters(query.Keys.Select(key => new UrlParameter(key, query[key].ToString())))
            };

            _requestedQueryRepository.Add(requestedQuery);

            var response = new Response { ContentType = "application/json" };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponseRepository.GetAll())
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Path.Equals(Request.Url.Path) && fakeRequest.Parameters.Equals(query))
                {
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
