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

        public FluentFaker WithParameters(string urlParameterString)
        {
            var parameters = urlParameterString.ParseParameters();

            return WithParameters(parameters);
        }

        public FluentFaker WithParameters(IEnumerable<UrlParameter> urlParameters)
        {
            _fakeRequestResponse.FakeRequest.Parameters = new UrlParameters(urlParameters);

            return this;
        }

        public FluentFaker WithFormParameters(string formParameterString)
        {
            var parameters = formParameterString.ParseParameters();

            return WithFormParameters(parameters);
        }

        public FluentFaker WithFormParameters(IEnumerable<UrlParameter> formParameters)
        {
            _fakeRequestResponse.FakeRequest.FormParameters = new UrlParameters(formParameters);

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