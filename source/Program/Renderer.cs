namespace Doom 
{
    public sealed class Renderer
    {
        public RenderingContext RenderingContext { get; }

        public Renderer(RenderingContext renderingContext) => RenderingContext = renderingContext;
    }
}