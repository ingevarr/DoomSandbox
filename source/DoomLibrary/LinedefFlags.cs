using System;

namespace DoomLibrary
{
    [Flags]
    public enum LinedefFlags : ushort
    {
        Blocking = 0,
        BlockingMonsters = 1,
        TwoSided = 1 << 1,
        NotPegTop = 1 << 2,
        NotPegBottom = 1 << 3,
        Secret = 1 << 4,
        BlockSound = 1 << 5,
        NotDraw = 1 << 6,
        Draw = 1 << 7
    }
}