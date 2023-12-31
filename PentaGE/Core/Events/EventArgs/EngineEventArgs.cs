﻿namespace PentaGE.Core.Events
{
    /// <summary>
    /// Base abstract class for events in the game engine.
    /// </summary>
    public abstract class EngineEventArgs : EventArgs
    {
        private readonly Action<EngineEventArgs> _onEvent;

        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        internal abstract EventCategory Category { get; }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        internal abstract EventType Type { get; }

        /// <summary>
        /// Checks if the event belongs to any of the specified categories.
        /// </summary>
        /// <param name="categories">The categories to check.</param>
        /// <returns><c>true</c> if the event belongs to any of the specified categories; otherwise, <c>false</c>.</returns>
        internal bool BelongsToCategory(EventCategory categories) =>
            (Category & categories) != EventCategory.None;

        /// <summary>
        /// Gets the window associated with the event.
        /// </summary>
        public Window Window { get; protected set; }

        /// <summary>
        /// Raises the event, invoking the associated event handler if subscribed.
        /// </summary>
        internal void RaiseEvent() => _onEvent?.Invoke(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        internal EngineEventArgs(Action<EngineEventArgs> onEvent, Window window)
        {
            _onEvent = onEvent;
            Window = window;
        }
    }
}