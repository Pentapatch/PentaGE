using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Manages a collection of hotkeys associated with specific key and modifier combinations.
    /// </summary>
    public sealed class HotKeyManager
    {
        private ModifierKey _modifiers = ModifierKey.None;
        private readonly Dictionary<(Key, ModifierKey), HotKey> _hotKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyManager"/> class.
        /// </summary>
        internal HotKeyManager()
        {
            _hotKeys = new Dictionary<(Key, ModifierKey), HotKey>();
        }

        /// <summary>
        /// Gets or sets wether to log the triggered events.
        /// </summary>
        internal bool LoggingEnabled { get; set; } = false;

        /// <summary>
        /// Gets the <see cref="HotKey"/> instance associated with the specified combination of key and modifier key(s).
        /// If the HotKey doesn't exist, a new one is created.
        /// </summary>
        /// <param name="key">The key that is associated with the HotKey.</param>
        /// <param name="modifierKeys">The modifier key(s) that are associated with the HotKey. Default is <see cref="ModifierKey.None"/>.</param>
        public HotKey this[Key key, ModifierKey modifierKeys = ModifierKey.None] =>
            CreateOrReturnHotKey(key, modifierKeys);

        /// <summary>
        /// Adds a new <see cref="HotKey"/> with the specified key and modifier key(s).
        /// If a <see cref="HotKey"/> with the same key and modifier key(s) combination already exists,
        /// the existing instance is returned.
        /// </summary>
        /// <param name="key">The key that is associated with the HotKey.</param>
        /// <param name="modifierKeys">The modifier key(s) that are associated with the HotKey.</param>
        /// <returns>
        /// The newly added <see cref="HotKey"/> instance if it doesn't exist; otherwise, the existing instance.
        /// </returns>
        public HotKey Add(Key key, ModifierKey modifierKeys)
        {
            // Return the HotKey if it already exists
            if (_hotKeys.TryGetValue((key, modifierKeys), out var existingHotKey)) 
                return existingHotKey;

            var hotKey = new HotKey(key, modifierKeys, this);
            _hotKeys.Add((key, modifierKeys), hotKey);
            return hotKey;
        }

        /// <summary>
        /// Removes the specified <see cref="HotKey"/>.
        /// </summary>
        /// <param name="hotKey">The HotKey to remove.</param>
        /// <returns><c>true</c> if the HotKey is successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(HotKey hotKey) =>
            _hotKeys.Remove((hotKey.Key, hotKey.ModifierKeys));

        /// <summary>
        /// Removes all hotkeys from the manager.
        /// </summary>
        public void Clear() => _hotKeys.Clear();

        /// <summary>
        /// Handles the event when a key is pressed with the associated modifier keys.
        /// </summary>
        /// <remarks>
        /// A HotKeyEvent will be triggered if a registered HotKey combination exists.
        /// </remarks>
        /// <param name="key">The key that was pressed.</param>
        /// <param name="mods">The modifier key(s) associated with the key press.</param>
        /// <param name="window">The window where the key press occurred.</param>
        internal void KeyPressed(Key key, ModifierKey mods, Window window)
        {
            // Add the modifier key to the current modifiers
            _modifiers |= mods;

            // Handle the key
            if (_hotKeys.TryGetValue((key, _modifiers), out var hotKey))
            {
                hotKey.TriggerEvent(window, LoggingEnabled);
            }
        }

        /// <summary>
        /// Handles the event when a key is released and removes associated modifier keys.
        /// </summary>
        /// <param name="key">The key that was released.</param>
        internal void KeyReleased(Key key)
        {
            // Remove the modifier key from the current modifiers
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                _modifiers &= ~ModifierKey.Shift;
            }
            else if (key == Key.LeftControl || key == Key.RightControl)
            {
                _modifiers &= ~ModifierKey.Control;
            }
            else if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                _modifiers &= ~ModifierKey.Alt;
            }
            else if (key == Key.LeftSuper || key == Key.RightSuper)
            {
                _modifiers &= ~ModifierKey.Super;
            }
        }

        /// <summary>
        /// Creates or returns an existing <see cref="HotKey"/> instance associated with the specified key and modifier key(s).
        /// If a HotKey with the specified parameters already exists, it is returned; otherwise, a new HotKey is created and added.
        /// </summary>
        /// <param name="key">The key that is associated with the HotKey.</param>
        /// <param name="modifierKeys">The modifier key(s) that are associated with the HotKey.</param>
        /// <returns>The existing or newly created <see cref="HotKey"/> instance.</returns>
        private HotKey CreateOrReturnHotKey(Key key, ModifierKey modifierKeys)
        {
            HotKey? hotKey = _hotKeys.FirstOrDefault(x => x.Value.Key == key && x.Value.ModifierKeys == modifierKeys).Value;
            if (hotKey is not null) return hotKey;
            return Add(key, modifierKeys);
        }

    }
}