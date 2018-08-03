﻿using System;
using System.Collections.Generic;
using TinyFakeHostHelper.Extensions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Fakers
{
    public class FluentFaker
    {
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;
        private FakeRequestResponse _fakeRequestResponse;

        public FluentFaker(IFakeRequestResponseRepository fakeRequestResponseRepository)
        {
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
        }

        public FluentFaker IfRequest(string path)
        {
            _fakeRequestResponse = new FakeRequestResponse
            {
                FakeRequest = new FakeRequest { Path = path }
            };

            return this;
        }

        public FluentFaker WithMethod(Method method)
        {
            _fakeRequestResponse.FakeRequest.Method = method;

            return this;
        }

        [Obsolete("Please use \"WithUrlParameters\" instead")]
        public FluentFaker WithParameters(string urlParameterString)
        {
            return WithUrlParameters(urlParameterString);
        }

        [Obsolete("Please use \"WithUrlParameters\" instead")]
        public FluentFaker WithParameters(IEnumerable<Parameter> urlParameters)
        {
            return WithUrlParameters(urlParameters);
        }

        public FluentFaker WithUrlParameters(string urlParameterString)
        {
            var parameters = urlParameterString.ParseParameters();

            return WithUrlParameters(parameters);
        }

        public FluentFaker WithUrlParameters(IEnumerable<Parameter> urlParameters)
        {
            _fakeRequestResponse.FakeRequest.UrlParameters = new Parameters(urlParameters);

            return this;
        }

        public FluentFaker WithFormParameters(string formParameterString)
        {
            var parameters = formParameterString.ParseParameters();

            return WithFormParameters(parameters);
        }

        public FluentFaker WithFormParameters(IEnumerable<Parameter> formParameters)
        {
            _fakeRequestResponse.FakeRequest.FormParameters = new Parameters(formParameters);

            return this;
        }

        public FluentFaker ThenReturn(FakeResponse fakeResponse)
        {
            _fakeRequestResponse.FakeResponse = fakeResponse;

            _fakeRequestResponseRepository.Add(_fakeRequestResponse);

            ClearRequestResponse();

            return this;
        }

        private void ClearRequestResponse()
        {
            _fakeRequestResponse = null;
        }
    }
}