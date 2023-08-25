namespace ConsoleIO
{
    public abstract class MenuItem
    {
        public bool Enabled { get; private set; } = true;

        public bool Selected { get; private set; } = false;

        public string Text { get; set; } = string.Empty;

        public Action? Action { get; set; } = null;

        public ConsoleKey? Shortcut { get; private set; } = null;

        public ConsoleColor? Foreground { get; private set; } = null;

        public ConsoleColor? Background { get; private set; } = null;

        internal MenuItem(string text, Action? action)
        {
            Text = text;
            Action = action;
        }

        protected virtual void ApplySettings(MenuItemSettings options)
        {
            Enabled = options.Enabled;
            Selected = options.Selected;
            Shortcut = options.Shortcut;
            Foreground = options.Foreground;
            Background = options.Background;
        }
    }
}