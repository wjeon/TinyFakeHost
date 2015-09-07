using System.Configuration;

namespace TinyFakeHostHelper.Configuration
{
    public class TinyFakeHostConfiguration : ITinyFakeHostConfiguration
    {
        private const int DefaultMaximumNumberOfPathSegments = 10;

        public int MaximumNumberOfUrlPathSegments
        {
            get
            {
                var maximumNumberOfPathSegments = ConfigurationManager.AppSettings["MaximumNumberOfPathSegments"];

                return string.IsNullOrEmpty(maximumNumberOfPathSegments)
                    ? DefaultMaximumNumberOfPathSegments : int.Parse(maximumNumberOfPathSegments);
            }
        }

        public bool RequestedQueryPrint { get; set; }
    }
}