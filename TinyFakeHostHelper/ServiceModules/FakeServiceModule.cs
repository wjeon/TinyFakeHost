﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Extensions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.ServiceModules
{
    public class FakeServiceModule
    {
        private readonly RequestDelegate _next;
        private readonly ITinyFakeHostConfiguration _tinyFakeHostConfiguration;
        private readonly IRequestedQueryRepository _requestedQueryRepository;
        private readonly IRequestValidator _requestValidator;

        public FakeServiceModule(RequestDelegate next, ITinyFakeHostConfiguration tinyFakeHostConfiguration, IRequestedQueryRepository requestedQueryRepository, IRequestValidator requestValidator)

        {
            _next = next;
            _tinyFakeHostConfiguration = tinyFakeHostConfiguration;
            _requestedQueryRepository = requestedQueryRepository;
            _requestValidator = requestValidator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            var method = (Method)Enum.Parse(typeof(Method), request.Method);
            var query = request.Query;
            var form = request.Form();
            var body = request.Body.AsString();

            var requestedQuery = new FakeRequest
            {
                Method = method,
                Path = request.Path,
                UrlParameters = query.ToParameters(),
                FormParameters = form.ToParameters(),
                Body = body
            };

            if (_tinyFakeHostConfiguration.RequestedQueryPrint)
                Console.WriteLine("Requested Query - {0}", requestedQuery);

            _requestedQueryRepository.Add(requestedQuery);

            var response = _requestValidator.GetValidatedFakeResponse(method, request.Path, query, form, body);

            context.Response.Headers.Clear();
            context.Response.ContentType = response.ContentType;
            if (response.Headers != null)
                response.Headers.Keys.ToList().ForEach(headerKey =>
                    context.Response.Headers.Add(headerKey, response.Headers[headerKey]));
            context.Response.StatusCode = response.StatusCode;

            await context.Response.WriteAsync(response.Body ?? string.Empty);
        }
    }
}