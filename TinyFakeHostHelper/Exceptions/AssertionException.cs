using System;

namespace TinyFakeHostHelper.Exceptions
{
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message)
        {
        }
    }
}