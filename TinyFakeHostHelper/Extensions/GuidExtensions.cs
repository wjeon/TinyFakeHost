using System;

namespace TinyFakeHostHelper.Extensions
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