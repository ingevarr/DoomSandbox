using System;

namespace DoomLibrary
{
    public sealed class Map
    {
        private readonly Vertex[] vertexes;
        private readonly Linedef[] linedefs;
    }

    internal readonly struct Vertex
    {
        public readonly short X;
        public readonly short Y;
    }

    internal readonly struct Linedef
    {
        public readonly ushort StartVertex;
        public readonly ushort EndVertex;
        
        public readonly LinedefFlags Flags;
    }

    [Flags]
    public enum LinedefFlags : ushort
    {
        Blocking         = 0,
        BlockingMonsters = 1,
        TwoSided         = 1 << 1,
        NotPegTop        = 1 << 2,
        NotPegBottom     = 1 << 3,
        Secret           = 1 << 4,
        BlockSound       = 1 << 5,
        NotDraw          = 1 << 6,
        Draw             = 1 << 7
    }
}