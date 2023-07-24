using GLFW;

namespace PentaGE.Core.Events
{
    internal sealed class EventHandler
    {
        private readonly PentaGameEngine _engine;
        private readonly Dictionary<GLFW.Window, Window> _registeredWindows = new();

        internal EventHandler(PentaGameEngine engine)
        {
            _engine = engine;
        }

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
        }

        private Window GetWindow(GLFW.Window windowHandle) => 
            _registeredWindows[windowHandle];

        private void KeyCallback(GLFW.Window windowHandle, Keys key, int scancode, InputState state, ModifierKeys mods)
        {
            // Handle the key event here
            if (state == InputState.Press)
            {
                // Key was pressed
            }
            else if (state == InputState.Release)
            {
                // Key was released
            }
            else if (state == InputState.Repeat)
            {
                // Key was held
            }
        }

        private void MousePositionCallback(GLFW.Window windowHandle, double xPos, double yPos)
        {
            
        }

        private void MouseButtonCallback(GLFW.Window windowHandle, MouseButton button, InputState state, ModifierKeys mods)
        {
            // Handle the mouse button event here
            if (state == InputState.Press)
            {
                // Mouse button was pressed
            }
            else if (state == InputState.Release)
            {
                // Mouse button was released
            }
        }
    }
}