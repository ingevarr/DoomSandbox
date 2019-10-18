using System;

namespace DoomLibrary.WadFile.Loading
{
    internal sealed class WadLoader
    {
        public Wad Load()
        {


            var header = ReadHeader();

            return new Wad(header);
        }

        private WadHeader ReadHeader()
        {
            return new WadHeader(WadType.IWad, UInt32.MaxValue, UInt32.MaxValue);
        }
    }
}