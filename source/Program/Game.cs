using Doom.GameMap;
using Doom.Input;
using Doom.WadFile;

namespace Doom 
{
    public sealed class Game
    {
        private Wad wad;
        private readonly MapLoader mapLoader;

        private readonly Renderer renderer;

        private readonly IInputTracker input;

        public Game(IInputTracker input, RenderingContext renderingContext)
        {
            mapLoader = new MapLoader(wad);
            this.input = input;
            renderer = new Renderer(renderingContext);
        }

        public void Initialize()
        {
            wad = Wad.FromFile("DOOM.WAD");
            var map = mapLoader.LoadByName("E1M1");
            renderer.InitializeResources(map);
        }

        public void Update(float deltaTime)
        {
        }

        public void Render()
        {
            renderer.Render();
        }


        public bool IsRunning { get; private set; } = true;
    }
}