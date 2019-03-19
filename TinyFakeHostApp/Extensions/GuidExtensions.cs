using System;

namespace TinyFakeHostApp.Extensions
{
    public static class GuidExtensions
    {
        public static string FirstSegment(this Guid guid)
        {
            var data = guid.ToString().Split('-');
            return data[0];
        }
    }
}