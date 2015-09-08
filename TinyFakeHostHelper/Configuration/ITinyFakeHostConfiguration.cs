namespace TinyFakeHostHelper.Configuration
{
    public interface ITinyFakeHostConfiguration
    {
        int MaximumNumberOfUrlPathSegments { get; }
        bool RequestedQueryPrint { get; set; }
    }
}