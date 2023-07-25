using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a key up event.
    /// </summary>
    public sealed class KeyUpEventArgs : EngineEventArgs
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
        internal KeyUpEventArgs(Action<EngineEventArgs> onEvent, Window window, Key key, ModifierKey modifierKeys) :
            base(onEvent, window)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }

        /// <summary>
        /// Checks whether any of the specified modifier keys were used during the key event.
        /// </summary>
        /// <param name="modifierKeys">The modifier keys to check.</param>
        /// <returns><c>true</c> if any of the specified modifier keys were used; otherwise, <c>false</c>.</returns>
        public bool ModifierKeyWasUsed(ModifierKey modifierKeys) =>
            (ModifierKeys & modifierKeys) != ModifierKey.None;

        /// <summary>
        /// Returns a string representation of the <see cref="KeyUpEventArgs"/> object, including the key and modifier keys.
        /// </summary>
        /// <returns>A string representing the key event with its associated data.</returns>
        public override string ToString() =>
            $"{{Key={Key}, ModifierKeys={ModifierKeys}}}";
    }
}