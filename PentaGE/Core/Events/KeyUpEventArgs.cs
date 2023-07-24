using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a key up event.
    /// </summary>
    public sealed class KeyUpEventArgs : EngineEvent
    {
        protected internal override EventCategory Category =>
            EventCategory.Key | EventCategory.Input;

        protected internal override EventType Type => EventType.KeyUp;

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
        /// <param name="window">The window associated with the event.</param>
        /// <param name="key">The key associated with the event.</param>
        /// /// <param name="modifierKeys">The modifier keys that were pressed in combination with the key event.</param>
        internal KeyUpEventArgs(Window window, Key key, ModifierKey modifierKeys) : base(window)
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
    }
}