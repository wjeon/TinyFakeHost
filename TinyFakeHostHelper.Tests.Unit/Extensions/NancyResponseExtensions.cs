using System.IO;
using System.Text;
using Nancy;

namespace TinyFakeHostHelper.Tests.Unit.Extensions
{
    public static class NancyResponseExtensions
    {
        public static bool IsEqualTo(this Response response, Response otherResponse)
        {
            return response.Content() == otherResponse.Content() &&
                   response.ContentType == otherResponse.ContentType &&
                   response.Headers == otherResponse.Headers &&
                   response.StatusCode == otherResponse.StatusCode &&
                   response.ReasonPhrase == otherResponse.ReasonPhrase;
        }
 
        public static string Content(this Response response)
        {
            string content;
            using (var ms = new MemoryStream())
            {
                response.Contents(ms);
                ms.Flush();
                ms.Position = 0;
                content = Encoding.UTF8.GetString(ms.ToArray());
            }
            return content;
        }
    }
}
