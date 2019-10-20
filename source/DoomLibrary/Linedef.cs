namespace DoomLibrary
{
    public readonly struct Linedef
    {
        public readonly ushort StartVertex;
        public readonly ushort EndVertex;
        public readonly LinedefFlags Flags;
        public readonly ushort LineType;
        public readonly ushort SectorTag;
        public readonly ushort FrontSidedef;
        public readonly ushort BackSidedef;
    }
}