using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a mouse down or up event.
    /// </summary>
    public sealed class MouseButtonEventArgs : EngineEventArgs
    {
        private readonly EventCategory _categories;
        private readonly EventType _type;

        /// <summary>
        /// Gets the category of the event.
        /// </summary>
        internal override EventCategory Category => _categories;

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        internal override EventType Type => _type;

        /// <summary>
        /// Gets the mouse button associated with the event.
        /// </summary>
        public MouseButton Button { get; init; }

        /// <summary>
        /// Gets the modifier keys that were pressed in combination with the key event.
        /// </summary>
        public ModifierKey ModifierKeys { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseButtonEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="button">The mouse button associated with the event.</param>
        /// <param name="modifierKeys">The modifier keys that were pressed in combination with the key event.</param>
        /// <param name="categories">The category or categories of the event.</param>
        /// <param name="type">The type of the event.</param>
        internal MouseButtonEventArgs(
            Action<EngineEventArgs> onEvent,
            Window window,
            MouseButton button,
            ModifierKey modifierKeys,
            EventCategory categories,
            EventType type) :
            base(onEvent, window)
        {
            Button = button;
            ModifierKeys = modifierKeys;
            _categories = categories;
            _type = type;
        }

        /// <summary>
        /// Checks whether any of the specified modifier keys were used during the key event.
        /// </summary>
        /// <param name="modifierKeys">The modifier keys to check.</param>
        /// <returns><c>true</c> if any of the specified modifier keys were used; otherwise, <c>false</c>.</returns>
        public bool ModifierKeyWasUsed(ModifierKey modifierKeys) =>
            (ModifierKeys & modifierKeys) != ModifierKey.None;

        /// <summary>
        /// Returns a string representation of the <see cref="MouseButtonEventArgs"/> object, including the mouse button and modifier keys.
        /// </summary>
        /// <returns>A string representing the mouse button event with its associated data.</returns>
        public override string ToString() =>
            $"{{Button={Button}, ModifierKeys={ModifierKeys}}}";
    }
}