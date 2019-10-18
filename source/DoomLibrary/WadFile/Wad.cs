using System.IO;

using DoomLibrary.WadFile.Loading;

namespace DoomLibrary.WadFile
{
    public sealed class Wad
    {
        public WadHeader Header { get; }

        internal Wad(WadHeader header) => Header = header;

        public static Wad FromFile(string fileName)
        {
            using var fileStream = File.OpenRead(fileName);
            return WadLoader.Load(fileStream);
        }
    }
}