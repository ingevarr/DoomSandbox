using Doom.GameMap;
using Doom.Input;
using Doom.WadFile;

using Veldrid;

namespace Doom 
{
    public sealed class Game
    {
        private Wad wad;
        private MapLoader mapLoader;

        private readonly Renderer renderer;

        private readonly IInputTracker input;

        public Game(IInputTracker inputTracker, RenderingContext renderingContext)
        {
            
            input = inputTracker;
            renderer = new Renderer(renderingContext);
        }

        public void Initialize()
        {
            wad = Wad.FromFile("DOOM.WAD");
            mapLoader = new MapLoader(wad);
            
            var map = mapLoader.LoadByName("E1M1");
            renderer.InitializeResources(map);
        }

        public void Update(float deltaTime)
        {
            if (input.GetKeyDown(Key.Escape))
                IsRunning = false;
        }

        public void Render()
        {
            renderer.Render();
        }


        public bool IsRunning { get; private set; } = true;
    }
}