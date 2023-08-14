using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a key down event.
    /// </summary>
    public class KeyDownEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the event category.
        /// </summary>
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Keyboard | EventCategory.Button;

        /// <summary>
        /// Gets the event type.
        /// </summary>
        internal override EventType Type => EventType.KeyDown;

        /// <summary>
        /// Gets the key associated with the event.
        /// </summary>
        public Key Key { get; init; }

        /// <summary>
        /// Gets a value indicating whether the key event is a repeat event.
        /// </summary>
        public bool IsRepeat { get; init; }

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
        /// <param name="isRepeat">A value indicating whether the key event is a repeat event.</param>
        internal KeyDownEventArgs(Action<EngineEventArgs> onEvent, Window window, Key key, ModifierKey modifierKeys, bool isRepeat) : base(onEvent, window)
        {
            Key = key;
            ModifierKeys = modifierKeys;
            IsRepeat = isRepeat;
        }

        /// <summary>
        /// Checks whether any of the specified modifier keys were used during the key event.
        /// </summary>
        /// <param name="modifierKeys">The modifier keys to check.</param>
        /// <returns><c>true</c> if any of the specified modifier keys were used; otherwise, <c>false</c>.</returns>
        public bool ModifierKeyWasUsed(ModifierKey modifierKeys) =>
            (ModifierKeys & modifierKeys) != ModifierKey.None;

        /// <summary>
        /// Returns a string representation of the <see cref="KeyDownEventArgs"/> object, including the key, modifier keys, and whether it is a key repeat event.
        /// </summary>
        /// <returns>A string representing the key event with its associated data.</returns>
        public override string ToString() =>
            $"{{Key={Key}, ModifierKeys={ModifierKeys}, IsRepeat={IsRepeat}}}";
    }
}