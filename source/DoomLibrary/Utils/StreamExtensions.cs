using System.IO;

namespace DoomLibrary.Utils
{
    public static class StreamExtensions
    {
        public static byte[] ReadAllToBytes(this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.GetBuffer();
        }
    }
}