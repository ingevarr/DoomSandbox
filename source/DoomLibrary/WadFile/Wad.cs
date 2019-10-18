namespace DoomLibrary.WadFile
{
    public sealed class Wad
    {
        public WadHeader Header { get; }

        public Wad(WadHeader header) => Header = header;
    }
}