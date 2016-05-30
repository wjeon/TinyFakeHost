using System;

namespace TinyFakeHostHelper.Supports
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now
        {
            get
            {
                return DateTimeOffset.Now;
            }
        }
        public DateTimeOffset UtcNow
        {
            get
            {
                return DateTimeOffset.UtcNow;
            }
        }
    }
}