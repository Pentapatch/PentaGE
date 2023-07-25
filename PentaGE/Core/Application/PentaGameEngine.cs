using GLFW;
using PentaGE.Core.Events;
using PentaGE.Core.Logging;
using PentaGE.Rendering;
using Serilog;

namespace PentaGE.Core
{

    // Im building my own game engine that targets Windows only and using only OpenGL through GLFW.
    // This is the main class that will be inherited by the concrete implementation (i.e. the game, or a game editor, etc.)
    // This is the main entry point for the engine.
    // My engine currently have two event management systems; one for GLFW events (window and input events), and one for
    // custom timing events (i.e. create and subscribe to events that occours at certain times, like every 5 seconds, etc.)
    // Currently, to access the GLFW events, users must access the Events property of the engine instance.
    // But to access timing events, users must access the Timing property of the engine instance.
    // Q: Should i consolidate these systems or keep them separate?
    // A: I think i should keep them separate, because they are different systems with different purposes.
    //    The GLFW event system is for handling window and input events, and the timing system is for handling
    //    timing events (i.e. events that occours at certain times, like every 5 seconds, etc.)
    //    The GLFW event system is for handling window and input events, and the timing system is for handling
    //    timing events (i.e. events that occours at certain times, like every 5 seconds, etc.)
    // But should i tie the events to a certain instance of a window?
    // A: I think i should, because it makes sense to tie the events to a window instance.
    //    But i should also have a global event system that handles events that are not tied to a window instance.
    // Alright, so i should keep the two systems separate, and somehow provide access to them through a Window instance?
    // A: Yes, i think that is the best way to do it.

    /// <summary>
    /// An abstract class representing the core of the Penta Game Engine.
    /// Concrete implementations of this class must provide specific functionality for Initialize, Shutdown, and Update methods.
    /// </summary>
    public abstract class PentaGameEngine
    {
        private readonly Timing _timing = new();
        private readonly Renderer _renderer;
        private readonly WindowManager _windowManager;
        private readonly EventManager _eventManager = new();
        private GameState _state = GameState.Initializing;

        /// <summary>
        /// Gets the timing manager for the game engine, responsible for handling frame timing and delta time calculation.
        /// </summary>
        public Timing Timing => _timing;

        /// <summary>
        /// Gets the window manager for the game engine, responsible for managing and interacting with windows.
        /// </summary>
        public WindowManager Windows => _windowManager;

        /// <summary>
        /// Gets the Renderer instance used by the game engine to manage rendering.
        /// </summary>
        internal Renderer Renderer => _renderer;

        /// <summary>
        /// Gets the EventManager responsible for handling input and window events in the game engine.
        /// </summary>
        public EventManager Events => _eventManager;

        /// <summary>
        /// Gets the current state of the game engine.
        /// </summary>
        public GameState State => _state;

        /// <summary>
        /// Called during game engine initialization to allow concrete implementations to set up the game.
        /// Implement this method to initialize the game, create resources, set up the scene, etc.
        /// If the implementation returns <c>false</c>, the engine startup will be canceled.
        /// </summary>
        /// <returns><c>true</c> if the game engine is successfully initialized; otherwise, <c>false</c>.</returns>
        protected abstract bool Initialize();

        /// <summary>
        /// Called during game engine termination to allow concrete implementations to clean up resources.
        /// Implement this method to release any acquired resources, perform cleanup tasks, etc.
        /// </summary>
        protected abstract void Shutdown();

        /// <summary>
        /// Called during each frame to update the game state.
        /// Implement this method to update game logic, handle user input, update entities, etc.
        /// </summary>
        protected abstract void Update();

        /// <summary>
        /// Initializes a new instance of the PentaGameEngine class.
        /// </summary>
        public PentaGameEngine()
        {
            _renderer = new(this);
            _windowManager = new(this);
        }

        /// <summary>
        /// Starts the game engine, initializing the engine and entering the game loop.
        /// </summary>
        /// <returns><c>true</c> if the engine starts successfully; otherwise, <c>false</c>.</returns>
        public bool Start()
        {
            // Initialize Serilog
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            // Log entry and exit
            Log.Information("Starting PentaGE.");
            var state = InitializeGameEngine();
            Log.Information("Terminated PentaGE.");

            return state;
        }

        /// <summary>
        /// Stops the game engine, terminating the engine and releasing resources.
        /// </summary>
        public void Stop()
        {
            Log.Information("Stopping the engine.");

            // Update the game state
            _state = GameState.Terminating;

            // Terminate Glfw
            Log.Information("Terminating GLFW.");
            Glfw.Terminate();

            // Allow the concrete implementation of the engine to unload resources
            using (var logger = Log.Logger.BeginPerfLogger("Terminating concrete implementation"))
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Initializes the game engine and allowing the concrete implementation to set up the game.
        /// If the initialization fails, this method should return <c>false</c> to cancel the engine startup.
        /// </summary>
        /// <returns><c>true</c> if the engine initializes successfully; otherwise, <c>false</c>.</returns>
        private bool InitializeGameEngine()
        {
            // Allow the concrete implementation of the engine to initialize
            using (Log.Logger.BeginPerfLogger("Initializing concrete implementation"))
            {
                if (!Initialize())
                {
                    Log.Fatal("The concrete implementation failed to initialize.");
                    return false;
                }
            }

            // Initialize GLFW (OpenGL Framework)
            // Must come after Initialize so the concrete implementation of the engine can add windows
            using (Log.Logger.BeginPerfLogger("Initializing GLFW"))
            {
                if (!Renderer.InitializeGLFW())
                {
                    Log.Fatal("Failed to initialize GLFW.");
                    return false;
                }
            }

            // Start the game loop
            using (Log.Logger.BeginPerfLogger("Entering game loop"))
            {
                RunGameLoop();
            }

            return true;
        }

        /// <summary>
        /// The main game loop, responsible for handling input, updating game state, rendering, and frame timing.
        /// </summary>
        private void RunGameLoop()
        {
            _state = GameState.Running;

            while (State == GameState.Running && !Windows.NoActiveWindows())
            {
                // Handle input events
                Events.Update();

                // Update game state
                Update();

                // Render graphics
                Renderer.Render();

                // Get the next frame
                Timing.NextFrame();
            }

            if (State != GameState.Terminating) Stop();
        }

    }
}