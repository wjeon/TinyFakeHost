using System;
using System.Linq;

namespace TinyFakeHostApp.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ErrorMessageBody(this Exception exception, string errorName)
        {
            return $"{errorName}({exception.StackTrace.Split(' ').Last()}): {exception.Message}";
        }
    }
}