using GLFW;

namespace PentaGE.Core
{
    /// <summary>
    /// Represents a manager class for handling windows in the game engine.
    /// </summary>
    public sealed class WindowManager
    {
        private readonly List<Window> _windows = new();
        private bool isInitialized = false;

        /// <summary>
        /// Gets a read-only list of windows managed by the window manager.
        /// </summary>
        public IReadOnlyList<Window> Windows => _windows.AsReadOnly();

        /// <summary>
        /// Gets the window at the specified index.
        /// </summary>
        /// <param name="index">The index of the window to retrieve.</param>
        /// <returns>The <see cref="Window"/> at the specified index.</returns>
        public Window this[int index] => _windows[index];

        /// <summary>
        /// Adds a new window to the window manager and initializes it if the manager is already initialized.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> instance to add.</param>
        /// <returns><c>true</c> if the window is successfully added; otherwise, <c>false</c>.</returns>
        public bool AddWindow(Window window)
        {
            _windows.Add(window);
            if (isInitialized && !window.Create())
            {
                return false;
                // TODO: Log failure
            }

            return true;
        }

        /// <summary>
        /// Removes a window from the window manager and terminates it to clean up resources.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> instance to remove.</param>
        /// <returns><c>true</c> if the window is successfully removed; otherwise, <c>false</c>.</returns>
        public bool RemoveWindow(Window window)
        {
            // Terminate the window to clean up resources
            window.Terminate();

            // Remove the window from the list
            return _windows.Remove(window);
        }

        /// <summary>
        /// Initializes the window manager and creates default windows if needed.
        /// This method also creates any added windows in the window list.
        /// </summary>
        /// <returns><c>true</c> if the window manager is successfully initialized; otherwise, <c>false</c>.</returns>
        internal bool Initialize()
        {
            CreateDefaultWindow();

            foreach (var window in _windows)
            {
                if (!window.Create())
                {
                    return false;
                    // TODO: Log failure
                };
            }

            isInitialized = true;
            return true;
        }

        /// <summary>
        /// Checks if there are no active windows.
        /// </summary>
        /// <returns><c>true</c> if there are no active windows; otherwise, <c>false</c>.</returns>
        internal bool NoActiveWindows()
        {
            if (!isInitialized) return true;

            int activeWindowCount = 0;
            foreach (var window in _windows)
            {
                if (!Glfw.WindowShouldClose(window.Handle)) activeWindowCount++;
            }

            return activeWindowCount == 0;
        }

        /// <summary>
        /// Creates a default window if no windows are currently added to the window manager.
        /// </summary>
        private void CreateDefaultWindow()
        {
            if (_windows.Count != 0) return;
            AddWindow(Window.CreateDefault());
        }

    }
}