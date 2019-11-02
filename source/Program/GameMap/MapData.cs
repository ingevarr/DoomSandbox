using Doom.DataPrimitives;

namespace Doom.GameMap
{
    public sealed class MapData
    {
        public Vertex[] Vertexes { get; }
        public short MinX;
        public short MinY;
        public short MaxX;
        public short MaxY;

        public Linedef[] Linedefs { get; }

        internal MapData(Vertex[] vertexes, Linedef[] linedefs)
        {
            Vertexes = vertexes;
            Linedefs = linedefs;
        }
    }
}