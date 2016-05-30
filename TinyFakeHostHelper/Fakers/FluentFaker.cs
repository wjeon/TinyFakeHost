using System;
using System.Collections.Generic;
using System.Linq;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.Extensions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Fakers
{
    public class FluentFaker
    {
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;
        private readonly ITinyFakeHostConfiguration _configuration;
        private FakeRequestResponse _fakeRequestResponse;
        private Guid? _lastCreatedFakeId;

        private const string MaxNumberOfSegmentsExceptionMessage =
            "The number of segments of the requested url path is bigger than MaximumNumberOfUrlPathSegments setting " +
            "in the configuration or bigger than the default maximum number of url path segments";

        public FluentFaker(IFakeRequestResponseRepository fakeRequestResponseRepository, ITinyFakeHostConfiguration configuration)
        {
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
            _configuration = configuration;
        }

        public FluentFaker IfRequest(string path)
        {
            GuardCondition_NumberOfPathSegmentsShouldNotBeBiggerThanMaxNumberOfPathSegmentsSetting(path);

            _fakeRequestResponse = new FakeRequestResponse
            {
                FakeRequest = new FakeRequest { Path = path }
            };

            return this;
        }

        private void GuardCondition_NumberOfPathSegmentsShouldNotBeBiggerThanMaxNumberOfPathSegmentsSetting(string path)
        {
            var segmentCount = path.TrimStart('/').Split('/').Count();

            if (segmentCount > _configuration.MaximumNumberOfUrlPathSegments)
                throw new MaximumNumberOfUrlPathSegmentsException(
                    MaxNumberOfSegmentsExceptionMessage
                );
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

            _lastCreatedFakeId = _fakeRequestResponse.Id;

            ClearRequestResponse();

            return this;
        }

        public Guid? LastCreatedFakeId
        {
            get { return _lastCreatedFakeId; }
        }

        public void DeleteFakeById(Guid id)
        {
            if (_lastCreatedFakeId == id)
                _lastCreatedFakeId = null;

            _fakeRequestResponseRepository.DeleteById(id);
        }

        public void DeleteLastCreatedFake()
        {
            if (!_lastCreatedFakeId.HasValue)
            {
                Console.WriteLine("Cannot not find last created fake");
                return;
            }

            _fakeRequestResponseRepository.DeleteById(_lastCreatedFakeId.Value);
            _lastCreatedFakeId = null;
        }

        private void ClearRequestResponse()
        {
            _fakeRequestResponse = null;
        }
    }
}