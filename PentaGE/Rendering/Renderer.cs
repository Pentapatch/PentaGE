using GLFW;
using PentaGE.Core;

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
                Console.WriteLine("Failed to initialize GLFW."); // TODO: Log
                return false;
            }

            // Set up GLFW versioning
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            // Initialize and create all windows
            if (!_engine.Windows.Initialize())
            {
                Console.WriteLine("Failed to create all GLFW windows."); // TODO: Log
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal void Render()
        {

        }
    }
}