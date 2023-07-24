namespace PentaGE.Core.Events
{
    /// <summary>
    /// Types of events.
    /// </summary>
    internal enum EventType
    {
        /// <summary>
        /// Default or undefined event type.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The window is being closed.
        /// </summary>
        WindowClose,

        /// <summary>
        /// The window gains focus.
        /// </summary>
        WindowFocus,

        /// <summary>
        /// The window loses focus.
        /// </summary>
        WindowLostFocus,

        /// <summary>
        /// The window is moved.
        /// </summary>
        WindowMoved,

        /// <summary>
        /// The window is resized.
        /// </summary>
        WindowResize,

        /// <summary>
        /// A key is pressed.
        /// </summary>
        KeyDown,

        /// <summary>
        /// A key is released.
        /// </summary>
        KeyUp,

        /// <summary>
        /// A mouse button is pressed.
        /// </summary>
        MouseButtonDown,

        /// <summary>
        /// A mouse button is released.
        /// </summary>
        MouseButtonUp,

        /// <summary>
        /// The mouse cursor is moved.
        /// </summary>
        MouseMoved,

        /// <summary>
        /// The mouse is scrolled.
        /// </summary>
        MouseScrolled,

        /// <summary>
        /// The mouse cursor enters the window.
        /// 
        MouseEntered,

        /// <summary>
        /// The mouse cursor leaves the window.
        /// 
        MouseLeft,
    }
}