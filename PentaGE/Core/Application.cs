using GLFW;
using PentaGE.Rendering;
using Monitor = GLFW.Monitor;

namespace PentaGE.Core
{
    public abstract class Application
    {
        private readonly Timing _timing = new();
        private readonly Renderer _renderer = new();
        private GameState _state = GameState.Initializing;

        internal Timing Timing => _timing;

        internal Renderer Renderer => _renderer;

        public GameState State => _state;

        public abstract void Initialize();

        public abstract void Shutdown();

        public abstract void Update();

        public void Start()
        {
            // Initialize GLFW (OpenGL Framework)
            InitializeGLFW();

            // Allow the concrete implementation of the engine to initialize
            Initialize();

            // Start the game loop
            Run();
        }

        public void Stop()
        {
            _state = GameState.Terminating;

            // Allow the concrete implementation of the engine to unload resources
            Shutdown();
        }

        private void Run()
        {
            _state = GameState.Running;
            while (State == GameState.Running)
            {
                // Handle input events
                // TODO: Create input handling system

                // Update game state
                Update();

                // Render graphics
                Renderer.Render();

                // Get the next frame
                Timing.NextFrame();
            }
        }

        private void InitializeGLFW()
        {
            if (!Glfw.Init())
            {
                Console.WriteLine("Failed to initialize GLFW.");
                return;
            }

            // Create a window
            var window = Glfw.CreateWindow(800, 600, "Hello GLFW", Monitor.None, Window.None);
            if (window == Window.None)
            {
                Console.WriteLine("Failed to create GLFW window.");
                Glfw.Terminate();
                
                return;
            }

            Glfw.MakeContextCurrent(window);

            // Continue with the rest of your code (e.g., render loop, input handling, etc.)

            // Terminate GLFW when done
            Glfw.Terminate();
        }

    }
}