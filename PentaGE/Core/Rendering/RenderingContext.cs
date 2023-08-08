using GLFW;
using Serilog;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents the rendering context associated with a GLFW window for handling graphics rendering in the PentaGameEngine.
    /// This class manages the initialization and disposal of the rendering context, ensuring proper OpenGL rendering for the associated window.
    /// </summary>
    internal class RenderingContext : IDisposable
    {
        private readonly GLFW.Window _windowHandle;

        /// <summary>
        /// Creates a new instance of the RenderingContext class for the specified window.
        /// The rendering context will be associated with the provided window's GLFW handle.
        /// </summary>
        /// <param name="window">The Window object to associate this rendering context with.</param>
        internal RenderingContext(Window window)
        {
            _windowHandle = window.Handle;

            if (!InitializeGLFWContext())
            {
                Log.Fatal("Failed to initialize GLFW context.");
                throw new System.Exception("Failed to initialize GLFW context.");
            }
        }

        /// <summary>
        /// Parameterless constructor to allow instantiation without a specific window. Use with caution.
        /// </summary>
        internal RenderingContext() { }

        /// <summary>
        /// Gets a value indicating whether the rendering context has been initialized and is ready for use.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Sets the rendering context associated with this instance as the current context and performs a buffer swap to display rendered content on the window.
        /// </summary>
        public void Use()
        {
            if (IsInitialized)
            {
                // Make sure the context is current (though usually GLFW handles this for you)
                Glfw.MakeContextCurrent(_windowHandle);

                // Swap the front and back buffers to display the rendered content
                Glfw.SwapBuffers(_windowHandle);
            }
        }

        /// <summary>
        /// Disposes the rendering context and resets it to an uninitialized state.
        /// </summary>
        public void Dispose()
        {
            if (IsInitialized)
            {
                Glfw.MakeContextCurrent(GLFW.Window.None);
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Initializes the GLFW context for rendering. This method should be called once after creating the RenderingContext.
        /// </summary>
        /// <returns><c>true</c> if GLFW context is successfully initialized; otherwise, <c>false</c>.</returns>
        private bool InitializeGLFWContext()
        {
            // Check if the context is already initialized
            if (IsInitialized) return true;

            // Set GLFW hints and create the OpenGL context
            Glfw.MakeContextCurrent(_windowHandle);

            IsInitialized = true;
            return true;
        }
    }
}