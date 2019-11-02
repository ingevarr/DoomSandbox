using System.Text;

namespace Doom.WadFile
{
    internal static class WadConstants
    {
        public static readonly Encoding WadEncoding = Encoding.UTF8;
        public const byte HeaderSizeInBytes = 12;
        public const byte LumpNameSizeInBytes = 8;
        public const byte WadTypeStringLength = 4;
    }
}