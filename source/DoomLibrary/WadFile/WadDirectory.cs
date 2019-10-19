namespace DoomLibrary.WadFile
{
    public sealed class WadDirectory
    {
        public uint LumpOffset { get; }
        public uint LumpSize { get; }
        public string LumpName { get; }

        public WadDirectory(uint lumpOffset, uint lumpSize, string lumpName)
        {
            LumpOffset = lumpOffset;
            LumpSize = lumpSize;
            LumpName = lumpName;
        }
    }
}