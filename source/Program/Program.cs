using System.Diagnostics;

using Doom.Input;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Doom
{
    public static class Program
    {
        public static void Main()
        {
            var windowCreateInfo = new WindowCreateInfo
                                   {
                                       X = 100,
                                       Y = 200,
                                       WindowWidth = 1280,
                                       WindowHeight = 800,
                                       WindowTitle = "Doom",
                                   };

            var window = VeldridStartup.CreateWindow(ref windowCreateInfo);
            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, GraphicsBackend.OpenGL);
            var resourceFactory = graphicsDevice.ResourceFactory;

            var inputTracker = new InputTracker();
            var renderingContext = new RenderingContext(graphicsDevice, resourceFactory, resourceFactory.CreateCommandList());
            var game = new Game(inputTracker, renderingContext);
            game.Initialize();

            RunLoop(window, inputTracker, game);
        }

        private static void RunLoop(Sdl2Window window, IInputUpdater inputUpdater, Game game)
        {
            var stopwatch = Stopwatch.StartNew();
            var previousElapsed = 0d;
            var newElapsed = stopwatch.Elapsed.TotalSeconds;
            while (window.Exists && game.IsRunning)
            {
                var deltaTime = (float) (newElapsed - previousElapsed);
                
                inputUpdater.Update(window.PumpEvents());
                game.Update(deltaTime);
                game.Render();

                previousElapsed = newElapsed;
                newElapsed = stopwatch.Elapsed.TotalSeconds;
            }
            stopwatch.Stop();
        }
    }
}
