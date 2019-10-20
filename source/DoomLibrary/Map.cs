using System.Linq;

using DoomLibrary.WadFile;

namespace DoomLibrary
{
    public sealed class Map
    {
        public Vertex[] Vertexes { get; }
        public Linedef[] Linedefs { get; }

        internal Map(Vertex[] vertexes, Linedef[] linedefs)
        {
            Vertexes = vertexes;
            Linedefs = linedefs;
        }
    }

    public class MapLoader
    {
        public Map LoadByName(string mapName)
        {
            return new Map(null, null);
        }
    }
}