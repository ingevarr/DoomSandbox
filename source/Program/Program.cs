using System;

using DoomLibrary.WadFile;

namespace Program
{
    public static class Program
    {
        public static void Main()
        {
            var wad = Wad.FromFile("DOOM.WAD");
        }
    }
}
