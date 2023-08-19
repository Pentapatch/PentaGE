using PentaGE.Common;
using Serilog;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Represents a hotkey combination associated with a specific key and modifier keys.
    /// HotKey instances can be subscribed to events or actions to handle trigger events
    /// when the associated key combination is pressed.
    /// </summary>
    public sealed class HotKey
    {
        private readonly HotKeyManager _manager;

        /// <summary>
        /// Gets the <see cref="Common.Key"/> that is associated with this HotKey.
        /// </summary>
        public Key Key { get; init; }

        /// <summary>
        /// Gets the <see cref="Common.ModifierKey"/>(s) that is associated with this HotKey.
        /// </summary>
        public ModifierKey ModifierKeys { get; init; }

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
        /// <param name="manager">The manager that created this instance.</param>
        internal HotKey(Key key, ModifierKey modifierKeys, HotKeyManager manager)
        {
            Key = key;
            ModifierKeys = modifierKeys;
            _manager = manager;
        }

        /// <summary>
        /// Remove this particular HotKey instance from the <see cref="HotKeyManager"/>.
        /// </summary>
        public void Remove() =>
            _manager.Remove(this);

        /// <summary>
        /// Triggers the HotKey event.
        /// </summary>
        /// <param name="window">The window that triggered the HotKey.</param>
        /// <param name="log">Whether to log the event.</param>
        internal void TriggerEvent(Window window, bool log = false)
        {
            if (Action is not null) Action();

            var eventArgs = new HotKeyEventArgs(OnEvent, window, Key, ModifierKeys, false);
            if (log) Log.Information($"Event [{eventArgs.Type}]: {eventArgs}");

            eventArgs.RaiseEvent();
        }

        /// <summary>
        /// Triggers the HotKey event and/or executes the associated action.
        /// </summary>
        private void OnEvent(EngineEventArgs e) =>
            Event?.Invoke(this, (HotKeyEventArgs)e);
    }
}