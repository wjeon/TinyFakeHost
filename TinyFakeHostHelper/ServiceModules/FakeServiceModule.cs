using System.Threading;
using Nancy;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Extensions;
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

            var routeBuilders = new [] { Delete, Get, Options, Patch, Post, Put };

            foreach (var routeBuilder in routeBuilders)
            {
                BuildRoutesForRequest(routeBuilder);
            }
        }

        private void BuildRoutesForRequest(RouteBuilder routeBuilder)
        {
            routeBuilder["/"] = p => ReturnFakeResult();

            var segments = string.Empty;

            for (var i = 0; i < _tinyFakeHostConfiguration.MaximumNumberOfUrlPathSegments; i++)
            {
                segments += "/{segment" + i + "}";
                routeBuilder[segments] = p => ReturnFakeResult();
            }
        }

        private dynamic ReturnFakeResult()
        {
            var query = Request.Query as DynamicDictionary;
            var form = Request.Form as DynamicDictionary;

            var requestedQuery = new FakeRequest
            {
                Path = Request.Url.Path,
                Parameters = query.ToParameters(),
                FormParameters = form.ToParameters()
            };

            _requestedQueryRepository.Add(requestedQuery);

            var response = new Response { ContentType = "application/json" };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponseRepository.GetAll())
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Path.Equals(Request.Url.Path) && fakeRequest.Parameters.Equals(query) && fakeRequest.FormParameters.Equals(form))
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
