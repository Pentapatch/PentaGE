using GLFW;
using Serilog;

namespace PentaGE.Core.Events
{
    public sealed class EventManager
    {
        private readonly Dictionary<GLFW.Window, Window> _registeredWindows = new();
        private readonly List<EngineEvent> _eventBuffer = new();

        public event EventHandler<KeyDownEventArgs>? KeyDown;
        public event EventHandler<KeyDownEventArgs>? KeyRepeat;
        public event EventHandler<KeyUpEventArgs>? KeyUp;

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

        internal void Update()
        {
            Glfw.PollEvents();

            ExecuteEvents();
        }

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

        private void OnKeyDown(EngineEvent e)
        {
            if (e is KeyDownEventArgs keyDownEventArgs)
            {
                KeyDown?.Invoke(this, keyDownEventArgs);
            }
        }

        private void OnKeyRepeat(EngineEvent e)
        {
            if (e is KeyDownEventArgs keyDownEventArgs)
            {
                KeyRepeat?.Invoke(this, keyDownEventArgs);
            }
        }

        private void OnKeyUp(EngineEvent e)
        {
            if (e is KeyUpEventArgs keyUpEventArgs)
            {
                KeyUp?.Invoke(this, keyUpEventArgs);
            }
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

        private void MousePositionCallback(GLFW.Window windowHandle, double xPos, double yPos)
        {
            
        }

        private void MouseButtonCallback(GLFW.Window windowHandle, MouseButton button, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                // Mouse button was pressed
            }
            else if (state == InputState.Release)
            {
                // Mouse button was released
            }
        }

        #endregion
    }
}