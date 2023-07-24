namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for the window closing event.
    /// </summary>
    public sealed class WindowClosingEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the event category.
        /// </summary>
        internal override EventCategory Category => 
            EventCategory.Window;

        /// <summary>
        /// Gets the event type.
        /// </summary>
        internal override EventType Type => 
            EventType.WindowClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowClosingEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        public WindowClosingEventArgs(Action<EngineEventArgs> onEvent, Window window) : base(onEvent, window)
        {
        }

        /// <summary>
        /// Returns a string representation of the event (for debugging purposes).
        /// </summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() => 
            $"{{No data}}";
    }
}
