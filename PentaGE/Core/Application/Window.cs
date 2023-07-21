﻿using GLFW;
using static OpenGL.GL;
using System.Drawing;

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

        /// <summary>
        /// Gets the handle of the GLFW window associated with this <see cref="Window"/> instance.
        /// </summary>
        /// <remarks>
        /// The window handle is a unique identifier that represents the window in the GLFW library.
        /// </remarks>
        internal GLFW.Window WindowHandle => _windowHandle;

        /// <summary>
        /// Gets the handle of the GLFW window associated with the shared window, if available.
        /// </summary>
        /// <remarks>
        /// The shared window handle is a unique identifier that represents the shared window's GLFW window handle.
        /// If no shared window is set, the handle will be GLFW.Window.None.
        /// </remarks>
        internal GLFW.Window SharedWindowHandle =>
            SharedWindow is Window window ? window.WindowHandle : GLFW.Window.None;

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
                    // TODO: Log failure
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
        /// Creates a new instance of the <see cref="Window"/> class with default settings.
        /// </summary>
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

        /// <summary>
        /// Creates a new <see cref="Window"/> instance with default settings and an optional window title.
        /// </summary>
        /// <param name="title">The title of the window (optional).</param>
        /// <returns>A new instance of the <see cref="Window"/> class with default settings.</returns>
        public static Window CreateDefault(string title = "") =>
            new() { Title = title != "" ? title : DEFAULT_TITLE };

        /// <summary>
        /// Checks if the window is created and valid.
        /// </summary>
        /// <returns><c>true</c> if the window is created; otherwise, <c>false</c>.</returns>
        private bool IsCreated() =>
            _windowHandle != GLFW.Window.None;

    }
}