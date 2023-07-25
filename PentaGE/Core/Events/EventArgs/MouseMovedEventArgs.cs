using System.Drawing;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a mouse moved event.
    /// </summary>
    public sealed class MouseMovedEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Mouse | EventCategory.Position;

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        internal override EventType Type =>
            EventType.MouseMoved;

        /// <summary>
        /// Gets the position of the mouse cursor when the event occurred.
        /// </summary>
        public Point Position { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMovedEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="position">The position of the mouse cursor when the event occurred.</param>
        public MouseMovedEventArgs(Action<EngineEventArgs> onEvent, Window window, Point position) :
            base(onEvent, window)
        {
            Position = position;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="MouseMovedEventArgs"/> object, including the mouse position.
        /// </summary>
        /// <returns>A string representing the mouse moved event with its associated data.</returns>
        public override string ToString() =>
            $"{{Position={Position}}}";
    }
}