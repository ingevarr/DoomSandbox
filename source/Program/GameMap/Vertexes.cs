using Doom.DataPrimitives;

namespace Doom.GameMap 
{
    public sealed class Vertexes
    {
        public Vertex[] Data { get; }

        public readonly short Left;
        public readonly short Bottom;
        public readonly short Right;
        public readonly short Top;

        public Vertexes(Vertex[] data,
                        short left,
                        short bottom,
                        short right,
                        short top)
        {
            Data = data;
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
        }
    }
}