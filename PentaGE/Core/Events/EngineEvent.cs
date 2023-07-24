namespace PentaGE.Core.Events
{
    /// <summary>
    /// Base abstract class for events in the game engine.
    /// </summary>
    public abstract class EngineEvent : EventArgs
    {
        private readonly Action<EngineEvent> _onEvent;

        #region Enums

        /// <summary>
        /// Flags to categorize the event.
        /// </summary>
        [Flags]
        protected internal enum EventCategory
        {
            /// <summary>
            /// Default or undefined category.
            /// </summary>
            Undefined = 0,

            /// <summary>
            /// Events related to windows.
            /// </summary>
            Window = 1 << 0,

            /// <summary>
            /// General input events.
            /// </summary>
            Input = 1 << 1,

            /// <summary>
            /// Key-related events.
            /// </summary>
            Key = 1 << 2,

            /// <summary>
            /// Mouse location-related events.
            /// </summary>
            MouseLocation = 1 << 3,

            /// <summary>
            /// Mouse button-related events.
            /// </summary>
            MouseButton = 1 << 4
        }

        /// <summary>
        /// Types of events.
        /// </summary>
        protected internal enum EventType
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
            /// The mouse is moved.
            /// </summary>
            MouseMoved,

            /// <summary>
            /// The mouse is scrolled.
            /// </summary>
            MouseScrolled
        }

#endregion

        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        protected internal abstract EventCategory Category { get; }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        protected internal abstract EventType Type { get; }

        /// <summary>
        /// Checks if the event belongs to the specified category.
        /// </summary>
        /// <param name="category">The category to check.</param>
        /// <returns><c>true</c> if the event belongs to the specified category; otherwise, <c>false</c>.</returns>
        internal bool BelongsToCategory(EventCategory category) =>
            Category.HasFlag(category);

        /// <summary>
        /// Gets the window associated with the event.
        /// </summary>
        public Window Window { get; protected set; }

        /// <summary>
        /// Raises the event, invoking the associated event handler if subscribed.
        /// </summary>
        internal void RaiseEvent() => _onEvent?.Invoke(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineEvent"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        internal EngineEvent(Action<EngineEvent> onEvent, Window window)
        {
            _onEvent = onEvent;
            Window = window;
        }
    }
}