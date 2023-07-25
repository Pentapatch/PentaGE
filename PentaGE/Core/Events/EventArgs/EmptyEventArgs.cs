namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for events that do not carry any data.
    /// </summary>
    public sealed class EmptyEventArgs : EngineEventArgs
    {
        private readonly EventCategory _eventCategories;
        private readonly EventType _eventType;

        /// <summary>
        /// Gets the event category.
        /// </summary>
        internal override EventCategory Category =>
            _eventCategories;

        /// <summary>
        /// Gets the event type.
        /// </summary>
        internal override EventType Type =>
            _eventType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="categories">The category or categories of the event.</param>
        /// <param name="type">The type of the event.</param>
        internal EmptyEventArgs(Action<EngineEventArgs> onEvent, Window window, EventCategory categories, EventType type) : base(onEvent, window)
        {
            _eventCategories = categories;
            _eventType = type;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="EmptyEventArgs"/> object.
        /// This method is used for debugging and logging purposes when the event does not carry specific data.
        /// The returned string will indicate that the event does not contain any data.
        /// </summary>
        /// <returns>A string representation indicating that the event does not contain any data.</returns>
        public override string ToString() =>
            $"{{No data}}";
    }
}