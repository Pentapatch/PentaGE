using GLFW;
using Serilog;
using System.Collections;

namespace PentaGE.Core
{
    /// <summary>
    /// Represents a manager class for handling windows in the game engine.
    /// </summary>
    public sealed class WindowManager : IEnumerable<Window>
    {
        private readonly List<Window> _windows = new();
        private readonly PentaGameEngine _engine;
        private bool _isInitialized = false;

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

        public WindowManager(PentaGameEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Adds a new window to the window manager and initializes it if the manager is already initialized.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> instance to add.</param>
        /// <returns><c>true</c> if the window is successfully added; otherwise, <c>false</c>.</returns>
        public bool AddWindow(Window window)
        {
            // Important note: Set the instance reference to the engine
            // This is so that Window factory methods can be used without having to specify the engine instance
            window._engine = _engine;

            _windows.Add(window);

            if (_isInitialized && !window.Create())
            {
                Log.Fatal($"Failed to add window '{window.Handle}'.");
                return false;
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
            AddDefaultWindow();

            foreach (var window in _windows)
            {
                if (!window.Create())
                {
                    Log.Fatal($"Failed to create window '{window.Handle}'.");
                    return false;
                };
            }

            _isInitialized = true;
            return true;
        }

        /// <summary>
        /// Checks if there are no active windows.
        /// </summary>
        /// <returns><c>true</c> if there are no active windows; otherwise, <c>false</c>.</returns>
        internal bool NoActiveWindows()
        {
            if (!_isInitialized) return true;

            int activeWindowCount = 0;
            foreach (var window in _windows)
            {
                if (!Glfw.WindowShouldClose(window.Handle)) activeWindowCount++;
            }

            return activeWindowCount == 0;
        }

        /// <summary>
        /// Adds a default window if no windows are currently added to the window manager.
        /// </summary>
        private void AddDefaultWindow()
        {
            if (_windows.Count != 0) return;
            AddWindow(Window.CreateDefault());
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of windows.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection of windows.</returns>
        public IEnumerator<Window> GetEnumerator() =>
            _windows.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection of windows.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection of windows.</returns>
        IEnumerator IEnumerable.GetEnumerator() =>
            _windows.GetEnumerator();

    }
}