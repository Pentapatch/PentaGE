namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for mouse left event.
    /// </summary>
    public sealed class MouseLeftEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Mouse | EventCategory.Hover;

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        internal override EventType Type =>
            EventType.MouseLeft;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseLeftEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        public MouseLeftEventArgs(Action<EngineEventArgs> onEvent, Window window) : base(onEvent, window)
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