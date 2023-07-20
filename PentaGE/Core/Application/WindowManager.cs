using GLFW;

namespace PentaGE.Core
{
    public sealed class WindowManager
    {
        private readonly List<Window> _windows = new();
        private bool isInitialized = false;

        public IReadOnlyList<Window> Windows => _windows.AsReadOnly();

        public Window this[int index] => _windows[index];

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

        public bool RemoveWindow(Window window)
        {
            // Terminate the window to clean up resources
            window.Terminate();

            // Remove the window from the list
            return _windows.Remove(window);
        }

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

        internal bool NoActiveWindows()
        {
            if (!isInitialized) return true;

            int activeWindowCount = 0;
            foreach (var window in _windows)
            {
                if (!Glfw.WindowShouldClose(window.WindowHandle)) activeWindowCount++;
            }

            return activeWindowCount == 0;
        }

        private void CreateDefaultWindow()
        {
            if (_windows.Count != 0) return;
            AddWindow(Window.CreateDefault());
        }

    }
}