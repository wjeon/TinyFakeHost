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
                BuildRoutesForRequest(routeBuilder, "/", "/{name*}");
            }
        }

        private void BuildRoutesForRequest(RouteBuilder routeBuilder, params string[] paths)
        {
            foreach (var path in paths)
            {
                routeBuilder[path] = p => ReturnFakeResult();
            }
        }

        private dynamic ReturnFakeResult()
        {
            var method = (Method)Enum.Parse(typeof(Method), Request.Method);
            var query = Request.Query as DynamicDictionary;
            var form = Request.Form as DynamicDictionary;

            var requestedQuery = new FakeRequest
            {
                Method = method,
                Path = Request.Url.Path,
                UrlParameters = query.ToParameters(),
                FormParameters = form.ToParameters()
            };

            if (_tinyFakeHostConfiguration.RequestedQueryPrint)
                Console.WriteLine("Requested Query - {0}", requestedQuery);

            _requestedQueryRepository.Add(requestedQuery);

            return _requestValidator.GetValidatedFakeResponse(method, Request.Url, query, form);
        }
    }
}
