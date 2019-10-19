using System;

using DoomLibrary.WadFile;

namespace Program
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var wad = Wad.FromFile("DOOM.WAD");
        }
    }
}
