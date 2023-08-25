namespace ConsoleIO
{
    public abstract class MenuItem
    {
        private readonly ConsoleMenu _owner;

        public bool Enabled { get; set; } = true;

        public bool Selected { get; internal set; } = false;

        public string Text { get; set; } = string.Empty;

        public ConsoleKey? Shortcut { get; set; } = null;

        public ConsoleColor? Foreground { get; set; } = null;

        public ConsoleColor? Background { get; set; } = null;

        public Action? Action { get; set; } = null;

        internal MenuItem(ConsoleMenu owner, string text, Action? action)
        {
            _owner = owner;
            Text = text;
            Action = action;
        }

        public void Select() => _owner.Select(this);

        public void Remove() => _owner.Remove(this);

        protected virtual void ApplySettings(MenuItemSettings options)
        {
            Enabled = options.Enabled;
            if (options.Selected) Select();
            Shortcut = options.Shortcut;
            Foreground = options.Foreground;
            Background = options.Background;
        }
    }
}