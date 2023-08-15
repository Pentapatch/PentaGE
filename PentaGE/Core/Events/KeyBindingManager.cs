namespace PentaGE.Core.Events
{
    /// <summary>
    /// Provides a central manager for creating, managing, and binding key and modifier key(s) combinations to specific actions or events.
    /// </summary>
    public sealed class KeyBindingManager
    {
        private readonly Dictionary<string, KeyBinding> _keyBindings = new();

        /// <summary>
        /// Gets the <see cref="EventManager"/> instance that manages the overall events and hotkeys for the application.
        /// </summary>
        internal EventManager EventManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingManager"/> class with a reference to the parent <see cref="EventManager"/>.
        /// </summary>
        /// <param name="eventManager">The <see cref="EventManager"/> instance to associate with the manager.</param>
        internal KeyBindingManager(EventManager eventManager)
        {
            EventManager = eventManager;
        }

        /// <summary>
        /// Provides access to a <see cref="KeyBinding"/> instance associated with the specified name.
        /// If the key binding doesn't exist, a new one is created.
        /// </summary>
        /// <param name="name">The name associated with the key binding.</param>
        public KeyBinding this[string name] =>
            CreateOrReturn(name);

        /// <summary>
        /// Provides access to a <see cref="KeyBinding"/> instance associated with the specified action.
        /// If the key binding doesn't exist, a new one is created.
        /// </summary>
        /// <param name="action">The action associated with the key binding.</param>
        public KeyBinding this[Action action] =>
            CreateOrReturn(action);

        /// <summary>
        /// Provides access to a <see cref="KeyBinding"/> instance associated with the specified event handler.
        /// If the key binding doesn't exist, a new one is created.
        /// </summary>
        /// <param name="event">The event handler associated with the key binding.</param>
        public KeyBinding this[EventHandler<HotKeyEventArgs> @event] =>
            CreateOrReturn(@event);

        /// <summary>
        /// Adds a new <see cref="KeyBinding"/> instance with the specified name.
        /// If a key binding with the same name already exists, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <param name="name">The name associated with the key binding.</param>
        /// <returns>The newly added <see cref="KeyBinding"/> instance.</returns>
        public KeyBinding Add(string name)
        {
            if (_keyBindings.ContainsKey(name))
            {
                throw new ArgumentException($"Key binding with name '{name}' already exists.");
            }

            return CreateOrReturn(name);
        }

        /// <summary>
        /// Adds a new <see cref="KeyBinding"/> instance associated with the specified action.
        /// If a key binding with the same name as the action's method name already exists, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <param name="action">The action associated with the key binding.</param>
        /// <returns>The newly added <see cref="KeyBinding"/> instance.</returns>
        public KeyBinding Add(Action action)
        {
            var name = action.Method.Name;

            if (_keyBindings.ContainsKey(name))
            {
                throw new ArgumentException($"Key binding with name '{name}' already exists.");
            }

            return CreateOrReturn(action);
        }

        /// <summary>
        /// Adds a new <see cref="KeyBinding"/> instance associated with the specified event handler.
        /// If a key binding with the same name as the event handler's method name already exists, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <param name="event">The event handler associated with the key binding.</param>
        /// <returns>The newly added <see cref="KeyBinding"/> instance.</returns>
        public KeyBinding Add(EventHandler<HotKeyEventArgs> @event)
        {
            var name = @event.Method.Name;

            if (_keyBindings.ContainsKey(name))
            {
                throw new ArgumentException($"Key binding with name '{name}' already exists.");
            }

            return CreateOrReturn(@event);
        }

        /// <summary>
        /// Removes the specified <see cref="KeyBinding"/> instance from the manager.
        /// The associated key binding is unbound before removal.
        /// </summary>
        /// <param name="keyBinding">The <see cref="KeyBinding"/> to remove.</param>
        /// <returns><c>true</c> if the key binding is successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(KeyBinding keyBinding)
        {
            keyBinding.Unbind();
            return _keyBindings.Remove(keyBinding.Name);
        }

        /// <summary>
        /// Clears all key bindings managed by the manager.
        /// All associated key bindings are unbound before removal.
        /// </summary>
        public void Clear()
        {
            foreach (var keyBinding in _keyBindings.Values)
            {
                keyBinding.Remove();
            }
        }

        /// <summary>
        /// Creates a new <see cref="KeyBinding"/> instance with the specified name or returns an existing instance with the same name.
        /// </summary>
        /// <param name="name">The name associated with the key binding.</param>
        /// <returns>The created or existing <see cref="KeyBinding"/> instance.</returns>
        private KeyBinding CreateOrReturn(string name)
        {
            if (_keyBindings.TryGetValue(name, out var existingKeyBinding))
            {
                return existingKeyBinding;
            }

            var keyBinding = new KeyBinding(name, this);

            _keyBindings.Add(name, keyBinding);

            return keyBinding;
        }

        /// <summary>
        /// Creates a new <see cref="KeyBinding"/> instance associated with the specified action, or returns an existing instance with the same name.
        /// </summary>
        /// <param name="action">The action associated with the key binding.</param>
        /// <returns>The created or existing <see cref="KeyBinding"/> instance.</returns>
        private KeyBinding CreateOrReturn(Action action)
        {
            var name = action.Method.Name;

            if (_keyBindings.TryGetValue(name, out var existingBinding))
            {
                return existingBinding;
            }

            var keyBinding = new KeyBinding(name, this)
            {
                Action = action
            };

            _keyBindings.Add(name, keyBinding);

            return keyBinding;
        }

        /// <summary>
        /// Creates a new <see cref="KeyBinding"/> instance associated with the specified event handler, or returns an existing instance with the same name.
        /// </summary>
        /// <param name="event">The event handler associated with the key binding.</param>
        /// <returns>The created or existing <see cref="KeyBinding"/> instance.</returns>
        private KeyBinding CreateOrReturn(EventHandler<HotKeyEventArgs> @event)
        {
            var name = @event.Method.Name;

            if (_keyBindings.TryGetValue(name, out var existingBinding))
            {
                return existingBinding;
            }

            var keyBinding = new KeyBinding(name, this);
            keyBinding.Event += @event;

            _keyBindings.Add(name, keyBinding);

            return keyBinding;
        }
    }
}