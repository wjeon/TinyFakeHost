using Nancy;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.ServiceModules
{
    public class FakeServiceModule : NancyModule
    {
        private readonly ITinyFakeHostConfiguration _tinyFakeHostConfiguration;
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;

        public FakeServiceModule(ITinyFakeHostConfiguration tinyFakeHostConfiguration, IFakeRequestResponseRepository fakeRequestResponseRepository)
        {
            _tinyFakeHostConfiguration = tinyFakeHostConfiguration;
            _fakeRequestResponseRepository = fakeRequestResponseRepository;

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
            var response = new Response { ContentType = "application/json" };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponseRepository.GetAll())
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Path.Equals(Request.Url.Path) && fakeRequest.Parameters.Equals(Request.Query))
                {
                    var fakeResponse = fakeRequestResponse.FakeResponse;

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
