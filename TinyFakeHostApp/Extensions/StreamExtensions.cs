using System.IO;
using System.Text;

namespace TinyFakeHostApp.Extensions
{
    public static class StreamExtensions
    {
        public static string AsString(this Stream stream, Encoding encoding = null)
        {
            if (!stream.CanSeek || !stream.CanRead)
                return null;

            string value;

            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                value = reader.ReadToEnd();
            }

            return value;
        }

        public static MemoryStream AsStream(this string content, Encoding encoding = null)
        {
            var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream, encoding ?? Encoding.UTF8))
            {
                writer.Write(content);
            }

            return stream;
        }
    }
}