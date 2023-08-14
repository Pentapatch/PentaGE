using PentaGE.Common;

namespace PentaGE.Core.Events
{
    public sealed class HotKey
    {
        private readonly Key _key;
        private readonly ModifierKey _modifierKeys;

        /// <summary>
        /// Occurs when this specific combination of key and modifier keys is pressed.
        /// </summary>
        public event EventHandler<HotKeyEventArgs>? Event;

        /// <summary>
        /// Gets or sets the action to be executed when this specific combination of key and modifier keys is pressed.
        /// </summary>
        public Action? Action { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKey"/> class with the specified key and modifier keys.
        /// </summary>
        /// <param name="key">The key that is associated with this HotKey.</param>
        /// <param name="modifierKeys">The modifier keys that are associated with this HotKey.</param>
        internal HotKey(Key key, ModifierKey modifierKeys)
        {
            _key = key;
            _modifierKeys = modifierKeys;
        }

        /// <summary>
        /// Triggers the HotKey event.
        /// </summary>
        internal void TriggerEvent(Window window)
        {
            if (Action is not null) Action();

            var eventArgs = new HotKeyEventArgs(OnEvent, window, _key, _modifierKeys, false);
            eventArgs.RaiseEvent();
        }

        /// <summary>
        /// Triggers the HotKey event and/or executes the associated action.
        /// </summary>
        private void OnEvent(EngineEventArgs e) =>
            Event?.Invoke(this, (HotKeyEventArgs)e);
    }
}