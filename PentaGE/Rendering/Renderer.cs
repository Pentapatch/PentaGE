using GLFW;
using static OpenGL.GL;
using PentaGE.Core;
using Serilog;

namespace PentaGE.Rendering
{
    /// <summary>
    /// The renderer responsible for handling graphics rendering in the PentaGameEngine.
    /// </summary>
    internal class Renderer
    {
        private readonly PentaGameEngine _engine;

        /// <summary>
        /// Creates a new instance of the Renderer class.
        /// </summary>
        /// <param name="engine">The PentaGameEngine instance associated with this Renderer.</param>
        public Renderer(PentaGameEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Initializes GLFW and sets up the necessary context hints for rendering.
        /// </summary>
        /// <returns><c>true</c> if GLFW is successfully initialized; otherwise, <c>false</c>.</returns>
        internal bool InitializeGLFW()
        {
            if (!Glfw.Init())
            {
                Log.Fatal("Failed to initialize GLFW");
                return false;
            }

            // Set up GLFW versioning
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            // Initialize and create all windows
            if (!_engine.Windows.Initialize())
            {
                Log.Fatal("Failed to create all GLFW windows.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal void Render()
        {
            glClearColor(MathF.Sin((float)_engine.Timing.TotalElapsedTime), MathF.Cos((float)_engine.Timing.TotalElapsedTime), 0, 1);
            glClear(GL_COLOR_BUFFER_BIT);
            Glfw.SwapBuffers(_engine.Windows[0].Handle);
        }
    }
}