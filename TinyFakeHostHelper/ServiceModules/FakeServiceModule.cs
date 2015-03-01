using Nancy;

namespace TinyFakeHostHelper.ServiceModules
{
    public class FakeServiceModule : NancyModule
    {
        public FakeServiceModule()
        {
            Get["/helloWorld"] = p => "Hello world";

            Get["/vendors/9876-5432-1098-7654/products"] = p =>
                {
                    Response response = @"[{""id"":460173,""name"":""Product A"",""type"":""chair"",""manufactureYear"":2014},{""id"":389317,""name"":""Product B"",""type"":""desk"",""manufactureYear"":2013}]";
                    response.ContentType = "application/json";
                    response.StatusCode = HttpStatusCode.OK;

                    return response;
                };
        }
    }
}
