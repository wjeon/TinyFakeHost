namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeRequest
    {
        public FakeRequest()
        {
            Parameters = new UrlParameters();
        }

        public string Path { get; set; }
        public UrlParameters Parameters { get; set; }

        public override string ToString()
        {
            return string.Format("Resource Path: {0}, Parameters: {1}", Path, Parameters);
        }
    }
}