using GLFW;
using PentaGE.Rendering;

namespace PentaGE.Core
{
    public abstract class PentaGameEngine
    {
        private readonly Timing _timing = new();
        private readonly Renderer _renderer;
        private readonly WindowManager _windowManager = new();
        private GameState _state = GameState.Initializing;

        internal Timing Timing => _timing;

        internal Renderer Renderer => _renderer;

        public WindowManager Windows => _windowManager;

        public GameState State => _state;

        protected abstract void Initialize();

        protected abstract void Shutdown();

        protected abstract void Update();

        public PentaGameEngine()
        {
            _renderer = new(this);
        }

        public bool Start()
        {
            // Allow the concrete implementation of the engine to initialize
            Initialize();

            // Initialize GLFW (OpenGL Framework)
            // Must come after Initialize so the concrete implementation of the engine can add windows
            if (!Renderer.InitializeGLFW())
            {
                // TODO: Log failure
                return false;
            }

            // Start the game loop
            Run();

            return true;
        }

        public void Stop()
        {
            // Update the game state
            _state = GameState.Terminating;

            // Terminate Glfw
            Glfw.Terminate();

            // Allow the concrete implementation of the engine to unload resources
            Shutdown();
        }

        private void Run()
        {
            _state = GameState.Running;
            while (State == GameState.Running && !Windows.NoActiveWindows())
            {
                // Handle input events
                // TODO: Create input handling system

                // Update game state
                Update();

                Glfw.PollEvents();

                // Render graphics
                Renderer.Render();

                // Get the next frame
                Timing.NextFrame();
            }

            if (State != GameState.Terminating) Stop();
        }

    }
}