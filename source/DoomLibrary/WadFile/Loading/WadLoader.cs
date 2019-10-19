using System;
using System.Collections.Generic;
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
            var directories = ReadDirectories(wadBytes[(int) header.FirstDirectoryOffset..], header.DirectoriesCount);

            return new Wad(header, directories);
        }

        private static WadHeader ReadHeader(in ReadOnlySpan<byte> headerBytes)
        {
            var type = ParseWadType(Encoding.UTF8.GetString(headerBytes[..4]));
            var directoriesCount = BitConverter.ToUInt32(headerBytes[4..8]);
            var firstDirectoryOffset = BitConverter.ToUInt32(headerBytes[8..]);

            return new WadHeader(type, directoriesCount, firstDirectoryOffset);
        }

        private static WadType ParseWadType(string wadTypeString)
            => wadTypeString switch
            {
                "IWAD" => WadType.IWad,
                "PWAD" => WadType.PWad,
                _ => throw new InvalidOperationException($"Can't parse Wad type {wadTypeString}")
            };
        
        private static IReadOnlyCollection<WadDirectory> ReadDirectories(in ReadOnlySpan<byte> directoriesBytes, uint directoriesCount)
        {
            var directories = new WadDirectory[directoriesCount];

            for (var i = 0; i < directories.Length; i++)
            {
                var directoryStart = i * 16;
                var directoryEnds = directoryStart + 16;
                directories[i] = ReadDirectory(directoriesBytes[directoryStart..directoryEnds]);
            }

            return directories;
        }

        private static WadDirectory ReadDirectory(in ReadOnlySpan<byte> directoryBytes)
        {
            var lumpOffset = BitConverter.ToUInt32(directoryBytes[..4]);
            var lumpSize = BitConverter.ToUInt32(directoryBytes[4..8]);
            var lumpName = Encoding.UTF8.GetString(directoryBytes[8..]);
            return new WadDirectory(lumpOffset, lumpSize, lumpName);
        }
    }
}