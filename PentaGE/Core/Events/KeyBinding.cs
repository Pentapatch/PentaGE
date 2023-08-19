using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Represents a key and modifier key(s) combination bound to a specific action or event.
    /// </summary>
    public sealed class KeyBinding
    {
        private readonly KeyBindingManager _manager;
        private bool _isListening;
        private Key _previousKey = Key.Unknown;
        private ModifierKey _previousModifierKeys = ModifierKey.None;

        /// <summary>
        /// Gets the key associated with this key binding.
        /// </summary>
        public Key Key { get; private set; } = Key.Unknown;

        /// <summary>
        /// Gets the modifier key(s) associated with this key binding.
        /// </summary>
        public ModifierKey ModifierKeys { get; private set; } = ModifierKey.None;

        /// <summary>
        /// Occurs when the key binding is triggered by a hotkey event.
        /// </summary>
        public event EventHandler<HotKeyEventArgs>? Event;

        /// <summary>
        /// Gets or sets the action associated with this key binding.
        /// </summary>
        /// <remarks>There can only be one action and the action gets triggered before the events.</remarks>
        public Action? Action { get; set; } = null;

        /// <summary>
        /// Gets the name of this key binding.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBinding"/> class with the specified name and manager.
        /// </summary>
        /// <param name="name">The name of the key binding.</param>
        /// <param name="manager">The <see cref="KeyBindingManager"/> instance associated with this key binding.</param>
        internal KeyBinding(string name, KeyBindingManager manager)
        {
            Name = name;
            _manager = manager;
        }

        /// <summary>
        /// Binds the specified key and modifier key(s) combination to this key binding.
        /// </summary>
        /// <param name="key">The key to bind.</param>
        /// <param name="modifierKeys">(Optional) The modifier key(s) to bind.</param>
        /// <returns>The current <see cref="KeyBinding"/> instance.</returns>
        public KeyBinding Bind(Key key, ModifierKey modifierKeys = ModifierKey.None)
        {
            // Unsubcribe from the HotKey event of the previous key binding (if any)
            if (Key != Key.Unknown)
            {
                _manager.EventManager.HotKeys[Key, ModifierKeys].Event -= KeyBinding_HotKey;
            }

            // Set the new key binding
            Key = key;
            ModifierKeys = modifierKeys;

            // Subscribe to the HotKey event for this key binding
            _manager.EventManager.HotKeys[key, modifierKeys].Event += KeyBinding_HotKey;

            return this;
        }

        /// <summary>
        /// Unbinds the current key binding.
        /// </summary>
        public void Unbind()
        {
            // Unsuscribe from the event if it is bound
            if (Key != Key.Unknown)
            {
                _manager.EventManager.HotKeys[Key, ModifierKeys].Event -= KeyBinding_HotKey;
            }
        }

        /// <summary>
        /// Initiates listening for a new key binding input.
        /// </summary>
        /// <returns>The current <see cref="KeyBinding"/> instance.</returns>
        public KeyBinding Listen()
        {
            if (_isListening) return this;

            _isListening = true;

            // Store the previous key (if any) so that it can be rebound
            // when StopListen() is called
            _previousKey = Key;
            _previousModifierKeys = ModifierKeys;

            // Unbind the current key
            Unbind();

            // Subscribe to the key down event
            _manager.EventManager.KeyDown += EventManager_KeyDown;

            return this; // Allow chaining
        }

        /// <summary>
        /// Stops listening for new key binding input.
        /// The previous key binding (if any) is restored.
        /// </summary>
        public void StopListening()
        {
            if (!_isListening) return;

            _isListening = false;

            // Rebind the previous key (if Listen() was called)
            if (_previousKey != Key.Unknown)
            {
                Bind(_previousKey, _previousModifierKeys);
            }

            // Unsubscribe from the key down event
            _manager.EventManager.KeyDown -= EventManager_KeyDown;
        }

        /// <summary>
        /// Removes the key binding from the manager and unbinds it.
        /// </summary>
        public void Remove() =>
            _manager.Remove(this);

        /// <summary>
        /// Handles the key down event when listening for a new key binding. Ignores modifier keys and binds the new key.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments containing information about the key down event.</param>
        private void EventManager_KeyDown(object? sender, KeyDownEventArgs e)
        {
            if (!_isListening) return;

            // Ignore modifier keys
            if (e.Key is Key.LeftShift or Key.RightShift
                or Key.LeftAlt or Key.RightAlt
                or Key.LeftControl or Key.RightControl
                or Key.LeftSuper or Key.RightSuper
                or Key.CapsLock or Key.NumLock)
            {
                return;
            }

            // Bind the new key
            Bind(e.Key, e.ModifierKeys);

            // Reset the previous key (to prevent it from being bound again)
            _previousKey = Key.Unknown;
            _previousModifierKeys = ModifierKey.None;

            // Stop listening
            StopListening();
        }

        /// <summary>
        /// Handles the HotKey event by triggering the associated <see cref="Action"/> (if any) followed by the subscribed <see cref="Event"/>.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments containing information about the HotKey event.</param>
        private void KeyBinding_HotKey(object? sender, HotKeyEventArgs e)
        {
            // Trigger the Action (if any) then the Events (if any)
            if (Action is not null) Action();
            Event?.Invoke(this, e);
        }
    }
}