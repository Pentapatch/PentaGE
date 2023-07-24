using GLFW;
using Serilog;
using System.Drawing;
using System.Numerics;

namespace PentaGE.Core.Events
{
    public sealed class EventManager
    {
        private readonly Dictionary<GLFW.Window, Window> _registeredWindows = new();
        private readonly List<EngineEventArgs> _eventBuffer = new();

        internal EventManager() { }

        /// <summary>
        /// Gets or sets the category or categories of events to log.
        /// </summary>
        internal EventCategory CategoriesToLog { get; set; } = EventCategory.Window;

        #region Internal methods

        internal void AddCallbacks(Window window)
        {
            Glfw.SetKeyCallback(window.Handle, KeyCallback);
            Glfw.SetCursorPositionCallback(window.Handle, MousePositionCallback);
            Glfw.SetMouseButtonCallback(window.Handle, MouseButtonCallback);
            Glfw.SetCursorEnterCallback(window.Handle, MouseEnterCallback);
            Glfw.SetScrollCallback(window.Handle, MouseScrollCallback);
            Glfw.SetCloseCallback(window.Handle, WindowClosingCallback);
            Glfw.SetWindowFocusCallback(window.Handle, WindowFocusCallback);
            Glfw.SetWindowIconifyCallback(window.Handle, WindowIconifyCallback);
            Glfw.SetWindowMaximizeCallback(window.Handle, WindowMaximizeCallback);
            Glfw.SetWindowSizeCallback(window.Handle, WindowSizeCallback);
            Glfw.SetWindowPositionCallback(window.Handle, WindowPositionCallback);

            _registeredWindows.Add(window.Handle, window);
        }

        internal void RemoveCallbacks(Window window)
        {
            if (!_registeredWindows.ContainsKey(window.Handle)) return;

            Glfw.SetKeyCallback(window.Handle, null!);
            Glfw.SetCursorPositionCallback(window.Handle, null!);
            Glfw.SetMouseButtonCallback(window.Handle, null!);
            Glfw.SetCursorEnterCallback(window.Handle, null!);
            Glfw.SetScrollCallback(window.Handle, null!);
            Glfw.SetCloseCallback(window.Handle, null!);
            Glfw.SetWindowFocusCallback(window.Handle, null!);
            Glfw.SetWindowIconifyCallback(window.Handle, null!);
            Glfw.SetWindowMaximizeCallback(window.Handle, null!);
            Glfw.SetWindowSizeCallback(window.Handle, null!);
            Glfw.SetWindowPositionCallback(window.Handle, null!);

            _registeredWindows.Remove(window.Handle);
        }
        internal void Update(bool pollEvents = true)
        {
            // Optionally poll events from Glfw
            if (pollEvents) Glfw.PollEvents();

            // Execute the event buffer
            ExecuteEvents();
        }

        #endregion

        #region Event declarations

        public event EventHandler<KeyDownEventArgs>? KeyDown;

        public event EventHandler<KeyDownEventArgs>? KeyRepeat;

        public event EventHandler<KeyUpEventArgs>? KeyUp;

        public event EventHandler<MouseButtonEventArgs>? MouseDown;

        public event EventHandler<MouseButtonEventArgs>? MouseUp;

        public event EventHandler<MouseMovedEventArgs>? MouseMoved;

        public event EventHandler<EmptyEventArgs>? MouseEntered;

        public event EventHandler<EmptyEventArgs>? MouseLeft;

        public event EventHandler<MouseScrolledEventArgs>? MouseScrolled;

        public event EventHandler<EmptyEventArgs>? WindowClosing;

        public event EventHandler<EmptyEventArgs>? WindowGotFocus;

        public event EventHandler<EmptyEventArgs>? WindowLostFocus;

        public event EventHandler<EmptyEventArgs>? WindowMinimized;

        public event EventHandler<EmptyEventArgs>? WindowMaximized;

        public event EventHandler<EmptyEventArgs>? WindowRestored;

        public event EventHandler<WindowResizedEventArgs>? WindowResized;

        public event EventHandler<WindowMovedEventArgs>? WindowMoved;

        #endregion

        #region Event handlers

        private void OnKeyDown(EngineEventArgs e) => InvokeEvent(e, KeyDown);

        private void OnKeyRepeat(EngineEventArgs e) => InvokeEvent(e, KeyRepeat);

        private void OnKeyUp(EngineEventArgs e) => InvokeEvent(e, KeyUp);

        private void OnMouseDown(EngineEventArgs e) => InvokeEvent(e, MouseDown);

        private void OnMouseUp(EngineEventArgs e) => InvokeEvent(e, MouseUp);

        private void OnMouseMoved(EngineEventArgs e) => InvokeEvent(e, MouseMoved);

        private void OnMouseEntered(EngineEventArgs e) => InvokeEvent(e, MouseEntered);

        private void OnMouseLeft(EngineEventArgs e) => InvokeEvent(e, MouseLeft);

        private void OnMouseScrolled(EngineEventArgs e) => InvokeEvent(e, MouseScrolled);

        private void OnWindowClosing(EngineEventArgs e) => InvokeEvent(e, WindowClosing);

        private void OnWindowGotFocus(EngineEventArgs e) => InvokeEvent(e, WindowGotFocus);

        private void OnWindowLostFocus(EngineEventArgs e) => InvokeEvent(e, WindowLostFocus);

        private void OnWindowMinimized(EngineEventArgs e) => InvokeEvent(e, WindowMinimized);

        private void OnWindowMaximized(EngineEventArgs e) => InvokeEvent(e, WindowMaximized);

        private void OnWindowRestored(EngineEventArgs e) => InvokeEvent(e, WindowRestored);

        private void OnWindowResized(EngineEventArgs e) => InvokeEvent(e, WindowResized);

        private void OnWindowMoved(EngineEventArgs e) => InvokeEvent(e, WindowMoved);

        #endregion

        #region Callbacks

        private void KeyCallback(GLFW.Window windowHandle, Keys key, int scancode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                _eventBuffer.Add(new KeyDownEventArgs(
                    OnKeyDown,
                    GetWindow(windowHandle),
                    (Common.Key)key,
                    (Common.ModifierKey)mods,
                    false));
            }
            else if (state == InputState.Release)
            {
                _eventBuffer.Add(new KeyUpEventArgs(OnKeyUp,
                    GetWindow(windowHandle),
                    (Common.Key)key,
                    (Common.ModifierKey)mods));

            }
            else if (state == InputState.Repeat)
            {
                _eventBuffer.Add(new KeyDownEventArgs(OnKeyRepeat,
                    GetWindow(windowHandle),
                    (Common.Key)key,
                    (Common.ModifierKey)mods,
                    true));
            }
        }

        private void MouseButtonCallback(GLFW.Window windowHandle, MouseButton button, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                _eventBuffer.Add(new MouseButtonEventArgs(
                    OnMouseDown,
                    GetWindow(windowHandle),
                    (Common.MouseButton)button,
                    (Common.ModifierKey)mods,
                    EventCategory.Input | EventCategory.Mouse | EventCategory.Button,
                    EventType.MouseButtonDown));
            }
            else if (state == InputState.Release)
            {
                _eventBuffer.Add(new MouseButtonEventArgs(
                    OnMouseUp,
                    GetWindow(windowHandle),
                    (Common.MouseButton)button,
                    (Common.ModifierKey)mods,
                    EventCategory.Input | EventCategory.Mouse | EventCategory.Button,
                    EventType.MouseButtonUp));
            }
        }

        private void MousePositionCallback(GLFW.Window windowHandle, double xPos, double yPos)
        {
            _eventBuffer.Add(new MouseMovedEventArgs(
                    OnMouseMoved,
                    GetWindow(windowHandle),
                    new((int)xPos, (int)yPos)));
        }

        private void MouseEnterCallback(GLFW.Window windowHandle, bool entered)
        {
            if (entered)
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnMouseEntered,
                    GetWindow(windowHandle),
                    EventCategory.Input | EventCategory.Mouse | EventCategory.Hover,
                    EventType.MouseEntered));
            }
            else
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnMouseLeft,
                    GetWindow(windowHandle),
                    EventCategory.Input | EventCategory.Mouse | EventCategory.Hover,
                    EventType.MouseLeft));
            }
        }

        private void MouseScrollCallback(GLFW.Window windowHandle, double xOffset, double yOffset)
        {
            _eventBuffer.Add(new MouseScrolledEventArgs(
                OnMouseScrolled,
                GetWindow(windowHandle),
                new Vector2((float)xOffset, (float)yOffset)));
        }

        private void WindowClosingCallback(GLFW.Window windowHandle)
        {
            _eventBuffer.Add(new EmptyEventArgs(
                OnWindowClosing,
                GetWindow(windowHandle),
                EventCategory.Window | EventCategory.Closing,
                EventType.WindowClosing));
        }

        private void WindowFocusCallback(GLFW.Window windowHandle, bool focused)
        {
            if (focused)
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnWindowGotFocus,
                    GetWindow(windowHandle),
                    EventCategory.Window | EventCategory.Focus,
                    EventType.WindowGotFocus));
            }
            else
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnWindowLostFocus,
                    GetWindow(windowHandle),
                    EventCategory.Window | EventCategory.Focus,
                    EventType.WindowLostFocus));
            }
        }

        private void WindowIconifyCallback(IntPtr windowHandle, bool focusing)
        {
            if (focusing)
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnWindowMinimized,
                    GetWindow(windowHandle),
                    EventCategory.Window | EventCategory.Iconify,
                    EventType.WindowMinimized));
            }
            else
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnWindowRestored,
                    GetWindow(windowHandle),
                    EventCategory.Window | EventCategory.Iconify,
                    EventType.WindowRestored));
            }
        }

        private void WindowMaximizeCallback(GLFW.Window windowHandle, bool maximizing)
        {
            if (maximizing)
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnWindowMaximized,
                    GetWindow(windowHandle),
                    EventCategory.Window | EventCategory.Maximize,
                    EventType.WindowMaximized));
            }
            else
            {
                _eventBuffer.Add(new EmptyEventArgs(
                    OnWindowRestored,
                    GetWindow(windowHandle),
                    EventCategory.Window | EventCategory.Maximize,
                    EventType.WindowRestored));
            }
        }

        private void WindowSizeCallback(GLFW.Window windowHandle, int width, int height)
        {
            _eventBuffer.Add(new WindowResizedEventArgs(
                OnWindowResized,
                GetWindow(windowHandle),
                new Point(width, height)));
        }

        private void WindowPositionCallback(GLFW.Window windowHandle, int xPos, int yPos)
        {
            _eventBuffer.Add(new WindowMovedEventArgs(
                OnWindowMoved,
                GetWindow(windowHandle),
                new Point(xPos, yPos)));
        }

        #endregion

        #region Private methods

        private void ExecuteEvents()
        {
            // Process the event buffer
            foreach (var currentEvent in _eventBuffer)
            {
                currentEvent.RaiseEvent();
                LogEvent(currentEvent);
            }

            // Clear the event buffer
            _eventBuffer.Clear();
        }

        private void LogEvent(EngineEventArgs currentEvent)
        {
            if (CategoriesToLog == EventCategory.None) return;

            if (currentEvent.BelongsToCategory(CategoriesToLog))
            {
                Log.Information($"Event [{currentEvent.Type}]: {currentEvent}");
            }
        }

        private void InvokeEvent<T>(EngineEventArgs e, EventHandler<T>? eventHandler)
        {
            // This method will greatly simply the creation of "On" methods
            if (e is T eventArgs) eventHandler?.Invoke(this, eventArgs);
        }

        private Window GetWindow(GLFW.Window windowHandle) =>
            _registeredWindows[windowHandle];

        private Window GetWindow(IntPtr windowHandle) =>
            _registeredWindows[new GLFW.Window(windowHandle)];

        #endregion
    }
}