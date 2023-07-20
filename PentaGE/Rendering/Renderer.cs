using GLFW;
using PentaGE.Core;

namespace PentaGE.Rendering
{
    internal class Renderer
    {
        private readonly PentaGameEngine _engine;

        public Renderer(PentaGameEngine engine)
        {
            _engine = engine;
        }

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

        internal void Render()
        {

        }


    }
}