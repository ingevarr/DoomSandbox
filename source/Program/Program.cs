using Doom.GameMap;
using Doom.WadFile;

namespace Doom
{
    public static class Program
    {
        public static void Main()
        {
            var wad = Wad.FromFile("DOOM.WAD");
            var mapLoader = new MapLoader(wad);

            var map = mapLoader.LoadByName("E1M1");
        }
    }
}
