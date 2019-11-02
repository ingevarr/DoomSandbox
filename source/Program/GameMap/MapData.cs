using Doom.DataPrimitives;

namespace Doom.GameMap
{
    public sealed class MapData
    {
        public Vertexes Vertexes { get; }
        public uint VertexesCount => (uint) Vertexes.Data.Length;


        public Linedef[] Linedefs { get; }

        internal MapData(Vertexes vertexes, Linedef[] linedefs)
        {
            Vertexes = vertexes;
            Linedefs = linedefs;
        }
    }
}