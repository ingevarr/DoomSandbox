using System;
using System.Collections.Generic;
using System.IO;

using static Doom.WadFile.WadConstants;

namespace Doom.WadFile.Loading
{
    internal static class WadLoader
    {
        public static Wad Load(Stream wadStream)
        {
            //var readAllToBytes = wadStream.ReadAllToBytes();
            if(!wadStream.CanSeek)
                throw new ArgumentException($"Can't seek on the {wadStream}");

            using var binaryReader = new BinaryReader(wadStream, WadEncoding);
            
            var header = ReadHeader(binaryReader);
            var directories = ReadDirectories(binaryReader, header);
            var lumpsBytes = ReadLumpsBytes(binaryReader, header);

            return new Wad(header, directories, lumpsBytes);
        }

        private static WadHeader ReadHeader(BinaryReader reader)
        {
            Span<byte> typeChars = stackalloc byte[WadTypeStringLength];
            reader.Read(typeChars);

            var typeString = WadEncoding.GetString(typeChars);
            var type = typeString switch
            {
                "IWAD" => WadType.IWad,
                "PWAD" => WadType.PWad,
                _ => throw new InvalidOperationException($"Can't parse Wad type {typeString}")
            };

            var directoriesCount = reader.ReadUInt32();
            var firstDirectoryOffset = reader.ReadUInt32();

            return new WadHeader(type, directoriesCount, firstDirectoryOffset);
        }

        private static IReadOnlyList<WadDirectory> ReadDirectories(BinaryReader reader, WadHeader header)
        {
            reader.BaseStream.Seek(header.FirstDirectoryOffset, SeekOrigin.Begin);

            var directories = new WadDirectory[header.DirectoriesCount];
            Span<char> nameSpan = stackalloc char[LumpNameSizeInBytes];
            for (var i = 0; i < header.DirectoriesCount; i++)
            {
                var lumpOffset = reader.ReadUInt32() - HeaderSizeInBytes;
                var lumpSize = reader.ReadUInt32();
                reader.Read(nameSpan);
                var name = string.Intern(new string(nameSpan).TrimEnd('\0'));
                directories[i] = new WadDirectory(lumpOffset, lumpSize, name);
            }
            return directories;
        }

        private static byte[] ReadLumpsBytes(BinaryReader reader, WadHeader header)
        {
            reader.BaseStream.Seek(HeaderSizeInBytes, SeekOrigin.Begin);
            return reader.ReadBytes((int) (header.FirstDirectoryOffset - HeaderSizeInBytes));
        }
    }
}