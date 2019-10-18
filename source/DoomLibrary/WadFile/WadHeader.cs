namespace DoomLibrary.WadFile
{
    public sealed class WadHeader
    {
        public WadType Type { get; }
        public uint DirectoriesCount { get; }
        public uint FirstDirectoryOffset { get; }

        public WadHeader(WadType type, uint directoriesCount, uint firstDirectoryOffset)
        {
            Type = type;
            DirectoriesCount = directoriesCount;
            FirstDirectoryOffset = firstDirectoryOffset;
        }
    }
}