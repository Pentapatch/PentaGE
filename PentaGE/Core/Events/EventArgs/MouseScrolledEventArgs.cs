using System.Numerics;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a mouse scrolled event.
    /// </summary>
    public sealed class MouseScrolledEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Mouse | EventCategory.Scroll;

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        internal override EventType Type =>
            EventType.MouseScrolled;

        /// <summary>
        /// Gets the scrolling offset for the mouse scrolled event.
        /// The offset represents the amount the mouse wheel was scrolled in both the X and Y directions.
        /// Positive values indicate scrolling up or to the right, while negative values indicate scrolling down or to the left.
        /// </summary>
        public Vector2 Offset { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMovedEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="offset">The scrolling offset in both the X and Y directions.</param>
        public MouseScrolledEventArgs(Action<EngineEventArgs> onEvent, Window window, Vector2 offset) :
            base(onEvent, window)
        {
            Offset = offset;
        }

        /// <summary>
        /// Returns a string representation of the event (for debugging purposes).
        /// </summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() =>
            $"{{Offset={Offset}}}";
    }
}