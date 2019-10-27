using System.Collections.Generic;
using System.IO;
using DoomLibrary.WadFile.Loading;

namespace DoomLibrary.WadFile
{
    public sealed class Wad
    {
        public WadHeader Header { get; }
        public IReadOnlyList<WadDirectory> Directories { get; }
        public byte[] LumpsBytes { get; }

        internal Wad(WadHeader header, IReadOnlyList<WadDirectory> directories, byte[] lumpsBytes)
        {
            Header = header;
            Directories = directories;
            LumpsBytes = lumpsBytes;
        }

        public static Wad FromFile(string fileName)
        {
            using var fileStream = File.OpenRead(fileName);
            return WadLoader.Load(fileStream);
        }
    }
}