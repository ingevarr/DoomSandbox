using System;
using System.Diagnostics;

using Doom.Input;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace Doom 
{
    public sealed class Application : IDisposable
    {
        private readonly IInputUpdater inputUpdater;

        private Game game;
        private Sdl2Window window;
        private RenderingContext renderingContext;
        private InputTracker inputTracker;

        public Application()
        {
            inputTracker = new InputTracker();
            inputUpdater = inputTracker;
        }

        public void Run()
        {
            Initialize();
            game.Initialize();
            RunLoop();
        }

        private void Initialize()
        {
            var windowCreateInfo = new WindowCreateInfo
                                   {
                                       X = 100,
                                       Y = 200,
                                       WindowWidth = 1280,
                                       WindowHeight = 800,
                                       WindowTitle = "Doom",
                                   };

            window = VeldridStartup.CreateWindow(ref windowCreateInfo);
            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, GraphicsBackend.OpenGL);
            
            var resourceFactory = new DisposeCollectorResourceFactory(graphicsDevice.ResourceFactory);
            renderingContext = new RenderingContext(graphicsDevice, resourceFactory, resourceFactory.CreateCommandList());
            game = new Game(inputTracker, renderingContext);
        }

        private void RunLoop()
        {
            var stopwatch = Stopwatch.StartNew();
            var previousElapsed = 0d;
            var newElapsed = stopwatch.Elapsed.TotalSeconds;
            while (window.Exists)
            {
                var deltaTime = (float) (newElapsed - previousElapsed);
                UpdateFrame(deltaTime);
                if (!game.IsRunning)
                    break;
                previousElapsed = newElapsed;
                newElapsed = stopwatch.Elapsed.TotalSeconds;
            }
            stopwatch.Stop();
        }

        private void UpdateFrame(float deltaTime)
        {
            inputUpdater.Update(window.PumpEvents());
            game.Update(deltaTime);
            game.Render(renderingContext);
        }

        public void Dispose()
        {
            
        }
    }
}