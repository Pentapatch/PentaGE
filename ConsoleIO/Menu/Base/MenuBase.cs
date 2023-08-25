namespace ConsoleIO
{
    public abstract class MenuBase<T> : IDisposable 
        where T : MenuBase<T>
    {
        protected readonly List<MenuItem> _items = new();
        private ConsoleColor _initialBackground;
        private ConsoleColor _initialForeground;
        private string _initialTitle = string.Empty;
        private bool _initialCursorVisible;

        public ConsoleColor Background { get; set; } = ConsoleColor.Black;

        public ConsoleColor Foreground { get; set; } = ConsoleColor.Gray;

        public ConsoleColor? SelectedBackgroundColor { get; set; } = null;

        public ConsoleColor? SelectedForegroundColor { get; set; } = null;

        public ConsoleColor? DisabledBackgroundColor { get; set; } = null;

        public ConsoleColor? DisabledForegroundColor { get; set; } = ConsoleColor.DarkGray;

        public ConsoleColor? MessageForeground { get; set; } = null;

        public ConsoleColor? MessageBackground { get; set; } = null;

        public char? SelectorChar { get; set; } = '>';

        public string Title { get; set; } = "ConsoleIO";

        public bool WrapArround { get; set; } = true;

        public MenuBase() => StoreInitialSettings();

        public MenuBase(Action<ConsoleMenuSettings>? settings)
        {
            StoreInitialSettings();

            if (settings is null) return;

            var menuSettings = new ConsoleMenuSettings();

            // Invoke the setup delegate
            settings(menuSettings);

            // Apply the options to the console
            ApplySettings(menuSettings);
        }

        ~MenuBase() => RestoreInitialSettings();

        protected void AddItem(MenuItem item) => _items.Add(item);

        #region Add message

        // name, (settings)
        public T AddMessage(string name, Action<MenuItemSettings>? settings = null) =>
            AddMessage(name, out _, settings);

        // name, out var, (settings)
        public T AddMessage(string name, out MenuMessage message, Action<MenuItemSettings>? settings = null)
        {
            message = new MenuMessage(name, settings);
            _items.Add(message);
            return (T)this;
        }

        #endregion

        #region Apply individual settings

        public T SetBackground(ConsoleColor color)
        {
            Background = color;
            Console.BackgroundColor = color;
            return (T)this;
        }

        public T SetForeground(ConsoleColor color)
        {
            Foreground = color;
            Console.ForegroundColor = color;
            return (T)this;
        }

        public T SetTitle(string title)
        {
            Title = title;
            Console.Title = title;
            return (T)this;
        }

        public T SetSelectedForegroundColor(ConsoleColor? color)
        {
            SelectedForegroundColor = color;
            return (T)this;
        }

        public T SetSelectedBackgroundColor(ConsoleColor? color)
        {
            SelectedBackgroundColor = color;
            return (T)this;
        }

        public T SetSelectorChar(char? selectorChar)
        {
            SelectorChar = selectorChar;
            return (T)this;
        }

        public T SetWrapArround(bool wrapArround)
        {
            WrapArround = wrapArround;
            return (T)this;
        }

        public T SetDisabledForeground(ConsoleColor? color)
        {
            DisabledForegroundColor = color;
            return (T)this;
        }

        public T SetDisabledBackground(ConsoleColor? color)
        {
            DisabledBackgroundColor = color;
            return (T)this;
        }

        public T SetMessageForeground(ConsoleColor? color)
        {
            MessageForeground = color;
            return (T)this;
        }

        public T SetMessageBackground(ConsoleColor? color)
        {
            MessageBackground = color;
            return (T)this;
        }

        #endregion

        public T Restore()
        {
            RestoreInitialSettings();
            return (T)this;
        }

        private void ApplySettings(ConsoleMenuSettings options)
        {
            SetBackground(options.Background);
            SetForeground(options.Foreground);
            SetTitle(options.Title);
            SetSelectedBackgroundColor(options.SelectedBackground);
            SetSelectedForegroundColor(options.SelectedForeground);
            SetSelectorChar(options.SelectorChar);
            SetWrapArround(options.WrapArround);
            Console.Clear();
        }

        private void StoreInitialSettings()
        {
            _initialBackground = Console.BackgroundColor;
            _initialForeground = Console.ForegroundColor;
            _initialTitle = Console.Title;
            _initialCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;
        }

        private void RestoreInitialSettings()
        {
            Console.BackgroundColor = _initialBackground;
            Console.ForegroundColor = _initialForeground;
            Console.Title = _initialTitle;
            Console.CursorVisible = _initialCursorVisible;
            Console.Clear();
        }

        public void Dispose() => RestoreInitialSettings();
    }
}