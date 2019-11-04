using Veldrid;

namespace Doom 
{
    public sealed class RenderingContext
    {
        public GraphicsDevice GraphicsDevice { get; }
        public ResourceFactory ResourceFactory { get; }
        public CommandList MainCommandList { get; }

        public RenderingContext(GraphicsDevice graphicsDevice, ResourceFactory resourceFactory, CommandList mainCommandList)
        {
            GraphicsDevice = graphicsDevice;
            ResourceFactory = resourceFactory;
            MainCommandList = mainCommandList;
        }
    }
}