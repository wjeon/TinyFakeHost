using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
{
    public static class HttpMethodExtensions
    {
        public static bool IsBodyAllowedMethod(this Method method)
        {
            return method == Method.POST || method == Method.PUT || method == Method.PATCH;
        }
    }
}