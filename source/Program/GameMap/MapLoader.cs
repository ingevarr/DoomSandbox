using System;
using System.Runtime.CompilerServices;

using Doom.DataPrimitives;
using Doom.WadFile;

namespace Doom.GameMap 
{
    public class MapLoader
    {
        private readonly Wad wad;

        public MapLoader(Wad wad) => this.wad = wad;

        public MapData LoadByName(string mapName)
        {
            var mapLumpIndex = FindMapLumpIndex(mapName);

            var vertexes = ReadVertexes(mapLumpIndex);
            var linedefs = ReadLinedefs(mapLumpIndex);

            return new MapData(vertexes, linedefs);
        }

        private Vertexes ReadVertexes(int mapIndex)
        {
            var vertexesDirectory = wad.Directories[mapIndex + (int)LumpIndexInMap.Vertexes];
            if(vertexesDirectory.LumpName != "VERTEXES")
                throw new Exception();

            var sizeOfVertex = Unsafe.SizeOf<Vertex>();
            var vertexes = new Vertex[vertexesDirectory.LumpSize / sizeOfVertex];

            var minX = short.MaxValue;
            var minY = short.MaxValue;
            var maxX = short.MinValue;
            var maxY = short.MinValue;

            var lump = GetLump((int) vertexesDirectory.LumpOffset, (int) vertexesDirectory.LumpSize);
            for (int i = 0, offset = 0; i < vertexes.Length; i++, offset = i * sizeOfVertex)
            {
                var vertexInLump = lump.Slice(offset, sizeOfVertex);
                var x = BitConverter.ToInt16(vertexInLump[..2]);
                var y = BitConverter.ToInt16(vertexInLump[2..]);
                vertexes[i] = new Vertex(x, y);

                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (x < minX) minX = x;
                if (y < minY) minY = y;
            }
            return new Vertexes(vertexes, minX, minY, maxX, maxY);
        }

        private Linedef[] ReadLinedefs(int mapIndex)
        {
            var linedefsDirectory = wad.Directories[mapIndex + (int)LumpIndexInMap.Linedefs];
            if(linedefsDirectory.LumpName != "LINEDEFS")
                throw new Exception();

            var sizeOfLinedef = Unsafe.SizeOf<Linedef>();
            var linedefs = new Linedef[linedefsDirectory.LumpSize / sizeOfLinedef];
            var lump = GetLump((int) linedefsDirectory.LumpOffset, (int) linedefsDirectory.LumpSize);
            for (int i = 0, offset = 0; i < linedefs.Length; i++, offset = i * sizeOfLinedef)
            {
                var linedefInLump = lump.Slice(offset, sizeOfLinedef);

                var startVertex = BitConverter.ToUInt16(linedefInLump[..2]);
                var endVertex = BitConverter.ToUInt16(linedefInLump[2..4]);
                var flags = BitConverter.ToUInt16(linedefInLump[4..6]);
                var lineType = BitConverter.ToUInt16(linedefInLump[6..8]);
                var sectorTag = BitConverter.ToUInt16(linedefInLump[8..10]);
                var frontSidedef = BitConverter.ToUInt16(linedefInLump[10..12]);
                var backSidedef = BitConverter.ToUInt16(linedefInLump[12..14]);

                linedefs[i] = new Linedef(startVertex, 
                                          endVertex, 
                                          (LinedefFlags)flags, 
                                          lineType, sectorTag, 
                                          frontSidedef, 
                                          backSidedef);
            }
            return linedefs;
        }

        private int FindMapLumpIndex(string mapName)
        {
            for (var i = 0; i < wad.Header.DirectoriesCount; i++)
            {
                var directory = wad.Directories[i];
                if (directory.LumpName == mapName)
                {
                    if (directory.LumpSize > 0)
                        throw new InvalidOperationException($"Can't load data for \"{mapName}\" because it is not marker directory");
                    return i;
                }
            }
            throw new InvalidOperationException($"Data for \"{mapName}\" not found");
        }

        private ReadOnlySpan<byte> GetLump(int offset, int length) 
            => wad.LumpsBytes.AsSpan(offset, length);
    }
}