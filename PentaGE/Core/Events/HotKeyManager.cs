using PentaGE.Common;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Manages a collection of hotkeys associated with specific key and modifier combinations.
    /// </summary>
    public sealed class HotKeyManager
    {
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
        /// </summary>
        /// <param name="key">The key that is associated with the HotKey.</param>
        /// <param name="modifierKeys">(Optional) The modifier key(s) that are associated with the HotKey.</param>
        /// <returns>The newly added <see cref="HotKey"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when a HotKey with the same key and modifier keys already exists.</exception>
        public HotKey Add(Key key, ModifierKey modifierKeys = ModifierKey.None)
        {
            if (_hotKeys.ContainsKey((key, modifierKeys)))
            {
                throw new ArgumentException($"A HotKey with the key {key} and modifier keys {modifierKeys} already exists.");
            }

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
        /// <param name="modifierKeys">The modifier key(s) associated with the key press.</param>
        /// <param name="window">The window where the key press occurred.</param>
        internal void KeyPressed(Key key, ModifierKey modifierKeys, Window window)
        {
            // Handle the key
            if (_hotKeys.TryGetValue((key, modifierKeys), out var hotKey))
            {
                hotKey.TriggerEvent(window, LoggingEnabled);
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
            if (_hotKeys.TryGetValue((key, modifierKeys), out var hotKey))
            {
                return hotKey;
            }

            return Add(key, modifierKeys);
        }
    }
}