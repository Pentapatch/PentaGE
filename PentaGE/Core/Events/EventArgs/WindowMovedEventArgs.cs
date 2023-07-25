using System.Drawing;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a window moved event.
    /// </summary>
    public sealed class WindowMovedEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the event category.
        /// </summary>
        internal override EventCategory Category => 
            EventCategory.Window | EventCategory.Position;

        /// <summary>
        /// Gets the event type.
        /// </summary>
        internal override EventType Type =>
            EventType.WindowMoved;

        /// <summary>
        /// Gets the location of the window after moving.
        /// </summary>
        public Point Location { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMovedEventArgs"/> class with the associated window and location.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="location">The new location of the window after moving.</param>
        public WindowMovedEventArgs(Action<EngineEventArgs> onEvent, Window window, Point location) : base(onEvent, window)
        {
            Location = location;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="WindowMovedEventArgs"/> object, including the location of the window.
        /// </summary>
        /// <returns>A string representing the window moved event with its associated data.</returns>
        public override string ToString() =>
            $"{{Location={Location}}}";
    }
}