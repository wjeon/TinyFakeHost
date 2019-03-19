using System;

namespace TinyFakeHostApp.Exceptions
{
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message)
        {
        }
    }
}