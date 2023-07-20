using GLFW;
using static OpenGL.GL;
using System.Drawing;

namespace PentaGE.Core
{
    public class Window
    {
        private const int DEFAULT_WIDTH = 1920;
        private const int DEFAULT_HEIGHT = 1080;
        private const string DEFAULT_TITLE = "Penta Game Engine";

        private const int VSYNC_ON = 1;
        private const int VSYNC_OFF = 0;

        private GLFW.Window _windowHandle = GLFW.Window.None;
        private Window? _sharedWindow = null;
        private Point _location;
        private Size _size;
        private string _title;
        private bool _resizable;
        private bool _focused;
        private bool _vSync;

        internal GLFW.Window WindowHandle => _windowHandle;

        internal GLFW.Window SharedWindowHandle =>
            SharedWindow is Window window ? window.WindowHandle : GLFW.Window.None;

        public Window? SharedWindow
        {
            get => _sharedWindow;
            set
            {
                _sharedWindow = value;

                if (!IsCreated()) return;
                // Re-create the window with the updated shared context
                if (!Create())
                {
                    // TODO: Log failure
                }
            }
        }

        public Point Location
        {
            get => _location;
            set
            {
                _location = value;

                if (!IsCreated()) return;
                Glfw.SetWindowPosition(_windowHandle, value.X, value.Y);
            }
        }

        public Size Size
        {
            get => _size;
            set
            {
                _size = value;

                if (!IsCreated()) return;
                Glfw.SetWindowSize(_windowHandle, value.Width, value.Height);
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;

                if (!IsCreated()) return;
                Glfw.SetWindowTitle(_windowHandle, value);
            }
        }

        public bool Resizable
        {
            get => _resizable;
            set
            {
                _resizable = value;

                if (!IsCreated()) return;
                Glfw.SetWindowAttribute(_windowHandle, WindowAttribute.Resizable, value);
            }
        }

        public bool Focused
        {
            get => _focused;
            set
            {
                _focused = value;

                if (!IsCreated()) return;
                Glfw.SetWindowAttribute(_windowHandle, WindowAttribute.Focused, value);
            }
        }

        public bool VSync
        {
            get => _vSync;
            set
            {
                _vSync = value;

                if (!IsCreated()) return;
                Glfw.MakeContextCurrent(_windowHandle); // Ensure the correct OpenGL context is active
                Glfw.SwapInterval(value ? VSYNC_ON : VSYNC_OFF);
            }
        }

        public Window()
        {
            _title = DEFAULT_TITLE;
            _size = new(DEFAULT_WIDTH, DEFAULT_HEIGHT);

            // Center the screen
            var screenSize = Glfw.PrimaryMonitor.WorkArea;
            var x = (screenSize.Width - Size.Width) / 2;
            var y = (screenSize.Height - Size.Height) / 2;
            _location = new Point(x, y);

            _resizable = false;
            _focused = true;
        }

        internal bool Create()
        {
            // Destroy the existing window if it's already created
            if (_windowHandle != GLFW.Window.None)
            {
                Glfw.DestroyWindow(_windowHandle);
            }

            // Set up the window
            Glfw.WindowHint(Hint.Focused, Focused);
            Glfw.WindowHint(Hint.Resizable, Resizable);

            // Create the window
            _windowHandle = Glfw.CreateWindow(Size.Width, Size.Height, Title, GLFW.Monitor.None, SharedWindowHandle);
            if (_windowHandle == GLFW.Window.None)
            {
                // TODO: Log failure
                return false;
            }

            Glfw.SetWindowPosition(_windowHandle, Location.X, Location.Y);

            Glfw.MakeContextCurrent(_windowHandle);
            Import(Glfw.GetProcAddress);

            glViewport(0, 0, Size.Width, Size.Height);
            Glfw.SwapInterval(_vSync ? VSYNC_ON : VSYNC_OFF);

            return true;
        }

        internal void Terminate()
        {
            if (_windowHandle == GLFW.Window.None) return;

            // Terminate the GLFW window
            Glfw.DestroyWindow(_windowHandle);
            _windowHandle = GLFW.Window.None;
        }

        public static Window CreateDefault(string title = "") =>
            new() { Title = title != "" ? title : DEFAULT_TITLE };

        private bool IsCreated() =>
            _windowHandle != GLFW.Window.None;

    }
}