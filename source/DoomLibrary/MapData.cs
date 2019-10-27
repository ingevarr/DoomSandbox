namespace DoomLibrary
{
    public sealed class MapData
    {
        public Vertex[] Vertexes { get; }
        public Linedef[] Linedefs { get; }

        internal MapData(Vertex[] vertexes, Linedef[] linedefs)
        {
            Vertexes = vertexes;
            Linedefs = linedefs;
        }
    }
}