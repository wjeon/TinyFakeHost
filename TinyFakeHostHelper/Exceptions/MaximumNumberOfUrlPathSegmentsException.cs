using System;

namespace TinyFakeHostHelper.Exceptions
{
    public class MaximumNumberOfUrlPathSegmentsException : Exception
    {
        public MaximumNumberOfUrlPathSegmentsException(string message) : base(message)
        {
        }
    }
}