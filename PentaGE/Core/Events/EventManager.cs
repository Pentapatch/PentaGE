using GLFW;
using System.Numerics;

namespace PentaGE.Core.Events
{
    public sealed class EventManager
    {
        private readonly Dictionary<GLFW.Window, Window> _registeredWindows = new();
        private readonly List<EngineEvent> _eventBuffer = new();

        internal EventManager() { }

        internal void AddCallbacks(Window window)
        {
            Glfw.SetKeyCallback(window.Handle, KeyCallback);
            Glfw.SetCursorPositionCallback(window.Handle, MousePositionCallback);
            Glfw.SetMouseButtonCallback(window.Handle, MouseButtonCallback);

            _registeredWindows.Add(window.Handle, window);
        }

        internal void RemoveCallbacks(Window window)
        {
            if (!_registeredWindows.ContainsKey(window.Handle)) return;

            Glfw.SetKeyCallback(window.Handle, null!);
            Glfw.SetCursorPositionCallback(window.Handle, null!);
            Glfw.SetMouseButtonCallback(window.Handle, null!);

            _registeredWindows.Remove(window.Handle);
        }

        internal void Update(bool pollEvents = true)
        {
            // Optionally poll events from Glfw
            if (pollEvents) Glfw.PollEvents();

            // Execute the event buffer
            ExecuteEvents();
        }

        #region Event declarations

        public event EventHandler<KeyDownEventArgs>? KeyDown;
        public event EventHandler<KeyDownEventArgs>? KeyRepeat;
        public event EventHandler<KeyUpEventArgs>? KeyUp;

        public event EventHandler<MouseDownEventArgs>? MouseDown;
        public event EventHandler<MouseUpEventArgs>? MouseUp;

        #endregion

        #region Event handlers

        private void OnKeyDown(EngineEvent e) => Invoke(e, KeyDown);

        private void OnKeyRepeat(EngineEvent e) => Invoke(e, KeyDown);

        private void OnKeyUp(EngineEvent e) => Invoke(e, KeyUp);

        private void OnMouseDown(EngineEvent e) => Invoke(e, MouseDown);

        private void OnMouseUp(EngineEvent e) => Invoke(e, MouseUp);

        #endregion

        #region Private methods

        private void ExecuteEvents()
        {
            // Process the event buffer
            foreach (var currentEvent in _eventBuffer)
            {
                currentEvent.RaiseEvent();
            }

            // Clear the event buffer
            _eventBuffer.Clear();
        }

        private void Invoke<T>(EngineEvent e, EventHandler<T>? eventHandler)
        {
            // This method will greatly simply the creation of "On" methods
            if (e is T eventArgs) eventHandler?.Invoke(this, eventArgs);
        }

        private Window GetWindow(GLFW.Window windowHandle) => 
            _registeredWindows[windowHandle];

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
                _eventBuffer.Add(new MouseDownEventArgs(
                    OnMouseDown,
                    GetWindow(windowHandle),
                    (Common.MouseButton)button,
                    (Common.ModifierKey)mods));
            }
            else if (state == InputState.Release)
            {
                _eventBuffer.Add(new MouseUpEventArgs(
                    OnMouseUp,
                    GetWindow(windowHandle),
                    (Common.MouseButton)button,
                    (Common.ModifierKey)mods));
            }
        }

        private void MousePositionCallback(GLFW.Window windowHandle, double xPos, double yPos)
        {
            
        }

        #endregion
    }
}