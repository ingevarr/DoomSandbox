using System;

namespace DoomLibrary
{
    [Flags]
    public enum LinedefFlags : ushort
    {
        Blocking         = 0,
        BlockingMonsters = 1,
        TwoSided         = 2,
        NotPegTop        = 4,
        NotPegBottom     = 8,
        Secret           = 16,
        BlockSound       = 32,
        NotDraw          = 64,
        Draw             = 128
    }
}