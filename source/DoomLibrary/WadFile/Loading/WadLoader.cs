using System;
using System.IO;
using System.Text;

using DoomLibrary.Utils;

namespace DoomLibrary.WadFile.Loading
{
    internal static class WadLoader
    {
        public static Wad Load(Stream wadStream)
        {
            ReadOnlySpan<byte> wadBytes = wadStream.ReadAllToBytes().AsSpan();

            var header = ReadHeader(wadBytes[..12]);

            return new Wad(header);
        }

        private static WadHeader ReadHeader(in ReadOnlySpan<byte> headerBytes)
        {
            var type = ParseWadType(Encoding.UTF8.GetString(headerBytes[..4]));
            var directoriesCount = BitConverter.ToUInt32(headerBytes[4..8]);
            var firstDirectoryOffset = BitConverter.ToUInt32(headerBytes[8..12]);

            return new WadHeader(type, directoriesCount, firstDirectoryOffset);
        }

        private static WadType ParseWadType(string wadTypeString) 
            => wadTypeString switch
            {
                "IWAD" => WadType.IWad,
                "PWAD" => WadType.PWad,
                _ => throw new InvalidOperationException($"Can't parse Wad type {wadTypeString}")
            };
    }
}