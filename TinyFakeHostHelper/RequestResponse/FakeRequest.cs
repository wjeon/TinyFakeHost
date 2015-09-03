namespace TinyFakeHostHelper.RequestResponse
{
    public class FakeRequest
    {
        public FakeRequest()
        {
            UrlParameters = new Parameters();
            FormParameters = new Parameters();
        }

        public string Path { get; set; }
        public Parameters UrlParameters { get; set; }
        public Parameters FormParameters { get; set; }

        public override string ToString()
        {
            return string.Format("Resource Path: {0}, UrlParameters: {1}, FormParameters: {2}", Path, UrlParameters, FormParameters);
        }
    }
}