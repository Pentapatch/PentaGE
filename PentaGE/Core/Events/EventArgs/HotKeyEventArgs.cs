using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a HotKey pressed event.
    /// </summary>
    public sealed class HotKeyEventArgs : KeyDownEventArgs
    {
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Keyboard | EventCategory.Button | EventCategory.HotKey;

        internal override EventType Type =>
             EventType.HotKeyPressed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="key">The key associated with the event.</param>
        /// <param name="modifierKeys">The modifier keys that were pressed in combination with the key event.</param>
        /// <param name="isRepeat">A value indicating whether the key event is a repeat event.</param>
        internal HotKeyEventArgs(Action<EngineEventArgs> onEvent, Window window, Key key, ModifierKey modifierKeys, bool isRepeat) :
            base(onEvent, window, key, modifierKeys, isRepeat)
        {

        }

    }
}