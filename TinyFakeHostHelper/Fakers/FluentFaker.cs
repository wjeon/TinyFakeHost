using System.Collections.Generic;
using System.Linq;
using Nancy.TinyIoc;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Exceptions;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Fakers
{
    public class FluentFaker
    {
        private readonly TinyIoCContainer _container;
        private FakeRequestResponse _fakeRequestResponse;
        private const string MaxNumberOfSegmentsExceptionMessage =
            "The number of segments of the requested url path is bigger than MaximumNumberOfUrlPathSegments setting " +
            "in the configuration or bigger than the default maximum number of url path segments";

        public FluentFaker(TinyIoCContainer container)
        {
            _container = container;
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
            var config = _container.Resolve<ITinyFakeHostConfiguration>();

            var segmentCount = path.TrimStart('/').Split('/').Count();

            if (segmentCount > config.MaximumNumberOfUrlPathSegments)
                throw new MaximumNumberOfUrlPathSegmentsException(
                    MaxNumberOfSegmentsExceptionMessage
                );
        }

        public FluentFaker WithParameters(string urlParameterString)
        {
            var parameters = ParseUrlParameters(urlParameterString);

            return WithParameters(parameters);
        }

        private static IEnumerable<UrlParameter> ParseUrlParameters(string urlParameterString)
        {
            var parameters = urlParameterString.Split('&')
                .Select(urlParam => urlParam.Split('='))
                .Select(param => new UrlParameter(param[0], param[1]));

            return parameters;
        }

        public FluentFaker WithParameters(IEnumerable<UrlParameter> urlParameters)
        {
            _fakeRequestResponse.FakeRequest.Parameters = new UrlParameters(urlParameters);

            return this;
        }

        public FluentFaker ThenReturn(FakeResponse fakeResponse)
        {
            _fakeRequestResponse.FakeResponse = fakeResponse;

            AddFakeRequestResponseToRepository();

            ClearRequestResponse();

            return this;
        }

        private void ClearRequestResponse()
        {
            _fakeRequestResponse = null;
        }

        private void AddFakeRequestResponseToRepository()
        {
            var fakeRequestResponseRepository = _container.Resolve<IFakeRequestResponseRepository>();

            fakeRequestResponseRepository.Add(_fakeRequestResponse);
        }
    }
}