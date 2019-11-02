namespace Doom.DataPrimitives
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
        
        public Linedef(ushort startVertex,
                       ushort endVertex,
                       LinedefFlags flags,
                       ushort lineType,
                       ushort sectorTag,
                       ushort frontSidedef,
                       ushort backSidedef)
        {
            StartVertex = startVertex;
            EndVertex = endVertex;
            Flags = flags;
            LineType = lineType;
            SectorTag = sectorTag;
            FrontSidedef = frontSidedef;
            BackSidedef = backSidedef;
        }
    }
}