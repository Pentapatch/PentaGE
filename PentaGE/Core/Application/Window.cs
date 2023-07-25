using GLFW;
using PentaGE.Core.Rendering;
using Serilog;
using System.Drawing;
using static OpenGL.GL;

namespace PentaGE.Core
{
    /// <summary>
    /// Represents a window in the application.
    /// </summary>
    public class Window
    {
        private const int DEFAULT_WIDTH = 1920;
        private const int DEFAULT_HEIGHT = 1080;
        private const string DEFAULT_TITLE = "Penta Game Engine";
        private const bool DEFAULT_RESIZABLE = false;

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

        private readonly PentaGameEngine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class with the specified parameters.
        /// This constructor is marked as internal, limiting the creation of instances to within the class assembly.
        /// The window is associated with the provided <paramref name="engine"/> and configured with the specified <paramref name="title"/> and <paramref name="size"/>.
        /// If the <paramref name="title"/> is not provided, the default window title will be used.
        /// If the <paramref name="size"/> is not provided, the window will have the default width and height.
        /// The window is centered on the primary monitor's work area, making it appear at the center of the screen.
        /// The window is set to be resizable by default, and it will be in focus upon creation.
        /// </summary>
        /// <param name="engine">The associated <see cref="PentaGameEngine"/> instance that manages the window.</param>
        /// <param name="title">The title of the window. If not provided, the default title will be used.</param>
        /// <param name="size">The size of the window. If not provided, the default width and height will be used.</param>
        internal Window(PentaGameEngine engine, string? title, Size? size)
        {
            _engine = engine;

            _title = title ?? DEFAULT_TITLE;
            _size = size ?? new(DEFAULT_WIDTH, DEFAULT_HEIGHT);

            // Center the screen
            var screenSize = Glfw.PrimaryMonitor.WorkArea;
            var x = (screenSize.Width - Size.Width) / 2;
            var y = (screenSize.Height - Size.Height) / 2;
            _location = new Point(x, y);

            _resizable = DEFAULT_RESIZABLE;
            _focused = true;
        }

        /// <summary>
        /// Gets or sets the rendering context associated with this window.
        /// The rendering context is responsible for handling graphics rendering operations for the window.
        /// </summary>
        /// <remarks>
        /// The rendering context is created and managed by the Window class when the window is created.
        /// It is used to render graphics and handle the OpenGL context for the associated window.
        /// </remarks>
        internal RenderingContext RenderingContext { get; private set; }

        /// <summary>
        /// Gets the handle of the GLFW window associated with this <see cref="Window"/> instance.
        /// </summary>
        /// <remarks>
        /// The window handle is a unique identifier that represents the window in the GLFW library.
        /// </remarks>
        internal GLFW.Window Handle => _windowHandle;

        /// <summary>
        /// Gets the handle of the GLFW window associated with the shared window, if available.
        /// </summary>
        /// <remarks>
        /// The shared window handle is a unique identifier that represents the shared window's GLFW window handle.
        /// If no shared window is set, the handle will be GLFW.Window.None.
        /// </remarks>
        internal GLFW.Window SharedWindowHandle =>
            SharedWindow is Window window ? window.Handle : GLFW.Window.None;

        /// <summary>
        /// Gets or sets a window that will share its OpenGL context with this window.
        /// </summary>
        /// <remarks>
        /// If a window is already created and a new shared window is set, the existing window will be recreated with the updated shared context.
        /// </remarks>
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
                    Log.Fatal($"Failed to recreate window '{Handle}' with shared context with '{SharedWindowHandle}'.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the window on the screen.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether the window can be resized by the user.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether the window has focus.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether vertical synchronization (VSync) is enabled.
        /// </summary>
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

        /// <summary>
        /// Removes the window from the WindowManager in the associated PentaGameEngine instance.
        /// The window will be unregistered from the InputHandler, and its GLFW window will be terminated.
        /// </summary>
        public void Remove() =>
            _engine.Windows.RemoveWindow(this);

        /// <summary>
        /// Maximizes the window if it is created and not already maximized.
        /// This operation will cause the window to occupy the entire screen, removing any window borders.
        /// </summary>
        public void Maximize()
        {
            if (!IsCreated()) return;
            Glfw.MaximizeWindow(_windowHandle);
        }

        /// <summary>
        /// Minimizes the window if it is created.
        /// This operation will cause the window to be iconified (minimized) to the taskbar or dock.
        /// </summary>
        public void Minimize()
        {
            if (!IsCreated()) return;
            Glfw.IconifyWindow(_windowHandle);
        }

        /// <summary>
        /// Restores the window if it is created and not already restored.
        /// </summary>
        public void Restore()
        {
            if (!IsCreated()) return;
            Glfw.RestoreWindow(_windowHandle);
        }

        /// <summary>
        /// Creates the GLFW window with the specified settings.
        /// </summary>
        /// <remarks>
        /// If a window already exists, the existing window will be destroyed before creating a new one.
        /// </remarks>
        /// <returns><c>true</c> if the window is successfully created; otherwise, <c>false</c>.</returns>
        internal bool Create()
        {
            // Destroy the existing window if it's already created
            if (_windowHandle != GLFW.Window.None)
            {
                Terminate();
            }

            // Set up the window
            Glfw.WindowHint(Hint.Focused, Focused);
            Glfw.WindowHint(Hint.Resizable, Resizable);

            // Create the window
            _windowHandle = Glfw.CreateWindow(Size.Width, Size.Height, Title, GLFW.Monitor.None, SharedWindowHandle);
            if (_windowHandle == GLFW.Window.None)
            {
                Log.Fatal("Failed to create a handle for a window.");
                return false;
            }

            Glfw.SetWindowPosition(_windowHandle, Location.X, Location.Y);

            // Set up the rendering context
            RenderingContext = new(this);

            Import(Glfw.GetProcAddress);

            glViewport(0, 0, Size.Width, Size.Height);
            Glfw.SwapInterval(_vSync ? VSYNC_ON : VSYNC_OFF);

            RegisterWindow();

            //_engine.Events.WindowClosing += Events_WindowClosing;

            return true;
        }

        // TODO: Will cause the engine to crash since the window is removed before the event is handled
        //private void Events_WindowClosing(object? sender, Events.EmptyEventArgs e)
        //{
        //    if (e.Window.Handle == Handle)
        //    {
        //        Remove();
        //    }
        //}

        /// <summary>
        /// Terminates the GLFW window associated with this instance and cleans up any resources.
        /// </summary>
        internal void Terminate()
        {
            if (_windowHandle == GLFW.Window.None) return;

            UnregisterWindow();

            RenderingContext.Dispose();

            // Terminate the GLFW window
            Glfw.DestroyWindow(_windowHandle);
            _windowHandle = GLFW.Window.None;
        }

        /// <summary>
        /// Checks if the window is created and valid.
        /// </summary>
        /// <returns><c>true</c> if the window is created; otherwise, <c>false</c>.</returns>
        private bool IsCreated() =>
            _windowHandle != GLFW.Window.None;

        /// <summary>
        /// Registers the window with the InputHandler in the associated PentaGameEngine instance.
        /// If the GLFW window has not been created, the method will log an error and return.
        /// </summary>
        private void RegisterWindow()
        {
            if (_windowHandle == GLFW.Window.None)
            {
                Log.Error("Attempted to register a window to the InputHandler that has not yet been created.");
                return;
            }

            _engine.Events.AddCallbacks(this);
        }

        /// <summary>
        /// Unregisters the window from the InputHandler in the associated PentaGameEngine instance.
        /// If the GLFW window has not been created, the method will log an error and return.
        /// </summary>
        private void UnregisterWindow()
        {
            if (_windowHandle == GLFW.Window.None)
            {
                Log.Error("Attempted to unregister a window to the InputHandler that has not yet been created.");
                return;
            }

            _engine.Events.RemoveCallbacks(this);
        }
    }
}