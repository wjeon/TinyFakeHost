using Nancy;

namespace TinyFakeHostHelper.ServiceModules
{
    public class FakeServiceModule : NancyModule
    {
        public FakeServiceModule()
        {
            Get["/"] = p => string.Empty;
        }
    }
}
