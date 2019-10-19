using System.Collections.Generic;
using System.IO;
using DoomLibrary.WadFile.Loading;

namespace DoomLibrary.WadFile
{
    public sealed class Wad
    {
        public WadHeader Header { get; }
        public IReadOnlyCollection<WadDirectory> Directories { get; }

        internal Wad(WadHeader header, IReadOnlyCollection<WadDirectory> directories)
        {
            Header = header;
            Directories = directories;
        }

        public static Wad FromFile(string fileName)
        {
            using var fileStream = File.OpenRead(fileName);
            return WadLoader.Load(fileStream);
        }
    }
}