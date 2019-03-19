using System;

namespace TinyFakeHostApp.RequestResponse
{
    public class FakeRequest
    {
        public FakeRequest()
        {
            Method = Method.GET;
            UrlParameters = new Parameters();
            FormParameters = new Parameters();
            Body = string.Empty;
        }

        public Method Method { get; set; }
        public string Path { get; set; }

        [Obsolete("Please use \"UrlParameters\" instead")]
        public Parameters Parameters
        {
            get { return UrlParameters; }
            set { UrlParameters = value; }
        }
        public Parameters UrlParameters { get; set; }
        public Parameters FormParameters { get; set; }
        public string Body { get; set; }

        public override string ToString()
        {
            return string.Format("Method: {0}, Resource Path: {1}, UrlParameters: {2}, FormParameters: {3}, Body: {4}", Method, Path, UrlParameters, FormParameters, Body);
        }
    }
}