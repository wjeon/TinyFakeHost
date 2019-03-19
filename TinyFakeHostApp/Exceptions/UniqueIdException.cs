using System;

namespace TinyFakeHostApp.Exceptions
{
    public class UniqueIdException : Exception
    {
        public UniqueIdException(string message) : base(message)
        {
        }
    }
}