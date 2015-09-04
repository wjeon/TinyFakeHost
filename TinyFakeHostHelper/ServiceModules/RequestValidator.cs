using System.Threading;
using Nancy;
using TinyFakeHostHelper.Persistence;

namespace TinyFakeHostHelper.ServiceModules
{
    public class RequestValidator : IRequestValidator
    {
        private readonly IFakeRequestResponseRepository _fakeRequestResponseRepository;

        public RequestValidator(IFakeRequestResponseRepository fakeRequestResponseRepository)
        {
            _fakeRequestResponseRepository = fakeRequestResponseRepository;
        }

        public Response GetValidatedFakeResponse(Url url, DynamicDictionary query, DynamicDictionary form)
        {
            var response = new Response { ContentType = "application/json" };

            var requestFound = false;

            foreach (var fakeRequestResponse in _fakeRequestResponseRepository.GetAll())
            {
                var fakeRequest = fakeRequestResponse.FakeRequest;

                if (fakeRequest.Path.Equals(url.Path) && fakeRequest.UrlParameters.Equals(query) &&
                    fakeRequest.FormParameters.Equals(form))
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