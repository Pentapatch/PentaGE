using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a key up event.
    /// </summary>
    public sealed class KeyUpEventArgs : EngineEvent
    {
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Keyboard | EventCategory.Button;

        internal override EventType Type => EventType.KeyUp;

        /// <summary>
        /// Gets the key associated with the event.
        /// </summary>
        public Key Key { get; init; }

        /// <summary>
        /// Gets the modifier keys that were pressed in combination with the key event.
        /// </summary>
        public ModifierKey ModifierKeys { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDownEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="key">The key associated with the event.</param>
        /// <param name="modifierKeys">The modifier keys that were pressed in combination with the key event.</param>
        internal KeyUpEventArgs(Action<EngineEvent> onEvent, Window window, Key key, ModifierKey modifierKeys) :
            base(onEvent, window)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }

        /// <summary>
        /// Checks whether a specific modifier key was used during the key event.
        /// </summary>
        /// <param name="modifierKey">The modifier key to check.</param>
        /// <returns><c>true</c> if the specified modifier key was used; otherwise, <c>false</c>.</returns>
        public bool ModifierKeyWasUsed(ModifierKey modifierKey) =>
            ModifierKeys.HasFlag(modifierKey);

        /// <summary>
        /// Returns a string representation of the event (for debugging purposes).
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{{Key={Key}, ModifierKeys={ModifierKeys}}}";
    }
}