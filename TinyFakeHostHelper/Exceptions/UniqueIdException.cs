using System;

namespace TinyFakeHostHelper.Exceptions
{
    public class UniqueIdException : Exception
    {
        public UniqueIdException(string message) : base(message)
        {
        }
    }
}