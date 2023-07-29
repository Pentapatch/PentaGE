using GLFW;
using Serilog;
using System.Collections;
using System.Drawing;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowManager"/> class with a reference to the <see cref="PentaGameEngine"/>.
        /// The window manager is responsible for handling windows in the game engine.
        /// </summary>
        /// <param name="engine">The <see cref="PentaGameEngine"/> instance that owns the window manager.</param>
        internal WindowManager(PentaGameEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Adds a new window to the window manager and initializes it if the manager is already initialized.
        /// If the <paramref name="title"/> is not provided, the window will have the default title.
        /// If the <paramref name="size"/> is not provided, the window will have the default width and height.
        /// </summary>
        /// <param name="title">The title of the window. If not provided, the default title will be used.</param>
        /// <param name="size">The size of the window. If not provided, the default width and height will be used.</param>
        /// <returns>The created <see cref="Window"/> instance.</returns>
        public Window Add(string? title = null, Size? size = null)
        {
            Window window = new(_engine, title, size);

            _windows.Add(window);

            if (_isInitialized && !window.Create())
            {
                Log.Fatal($"Failed to add window '{window.Handle}'.");
                throw new System.Exception($"Failed to add window '{window.Handle}'.");
            }

            return window;
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
        /// Additional calls to this method will return <c>true</c> immediately.
        /// </summary>
        /// <returns><c>true</c> if the window manager is successfully initialized; otherwise, <c>false</c>.</returns>
        internal bool Initialize()
        {
            if (_isInitialized) return true;

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
            Add();
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