using System;

namespace TinyFakeHostApp.Supports
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
        DateTimeOffset UtcNow { get; }
    }
}