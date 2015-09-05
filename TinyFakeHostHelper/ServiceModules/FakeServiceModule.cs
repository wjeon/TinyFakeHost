using System;
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
        private readonly IRequestedQueryRepository _requestedQueryRepository;
        private readonly IRequestValidator _requestValidator;

        public FakeServiceModule(ITinyFakeHostConfiguration tinyFakeHostConfiguration, IRequestedQueryRepository requestedQueryRepository, IRequestValidator requestValidator)
        {
            _tinyFakeHostConfiguration = tinyFakeHostConfiguration;
            _requestedQueryRepository = requestedQueryRepository;
            _requestValidator = requestValidator;

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
                UrlParameters = query.ToParameters(),
                FormParameters = form.ToParameters()
            };

            if (_tinyFakeHostConfiguration.RequestedQueryPrint)
                Console.WriteLine("Requested Query - {0}", requestedQuery);

            _requestedQueryRepository.Add(requestedQuery);

            return _requestValidator.GetValidatedFakeResponse(Request.Url, query, form);
        }
    }
}
