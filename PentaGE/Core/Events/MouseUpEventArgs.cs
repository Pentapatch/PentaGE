using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a mouse up event.
    /// </summary>
    public sealed class MouseUpEventArgs : MouseButtonEventArgs
    {
        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        internal override EventCategory Category => 
            EventCategory.Input | EventCategory.Mouse | EventCategory.Button;

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        internal override EventType Type => 
            EventType.MouseButtonUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseUpEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="button">The mouse button associated with the event.</param>
        /// <param name="modifierKeys">The modifier keys that were pressed in combination with the key event.</param>
        public MouseUpEventArgs(Action<EngineEvent> onEvent, Window window, MouseButton button, ModifierKey modifierKeys) : 
            base(onEvent, window, button, modifierKeys)
        {

        }
    }
}