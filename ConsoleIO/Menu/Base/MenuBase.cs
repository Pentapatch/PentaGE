namespace ConsoleIO
{
    // Design goal pseudo code:
    //private void EnterTestMenu() => new Menu()
    //    .SetBackground(ConsoleColor.Blue)
    //    .SetForeground(ConsoleColor.Gray)
    //    .SetHeader("Test menu")
    //    .AddMessage("Settings")
    //    .AddCheckBox("Enable A", out var checkBoxA, settings => 
    //    { 
    //        settings.Enabled = false; 
    //        settings.Checked = true; 
    //    })
    //    .AddCheckBox("Enable B", out var checkBoxB)
    //    .AddInput("Target", out var inputField, settings => 
    //    { 
    //        settings.Value = "C:\\Users\\User\\Desktop\\test.txt"; 
    //    })
    //    .AddMessage("Actions")
    //    .Add("Execute", settings =>
    //    {
    //        settings.Shorcut = ConsoleKey.F5;
    //    },
    //    () =>
    //    {
    //        if (checkBoxA.Checked)
    //            Console.WriteLine($"A is enabled, executing at {inputField.Value}");
    //    })
    //    .Add("Override test", TestAction1)
    //    .Add("Exit", () =>
    //    {
    //        Environment.Exit(0);
    //    })
    //    .Select(inputField) // Sets the item to be selected by default
    //    .Enter(() =>
    //    {
    //        Console.WriteLine$"Menu exited");
    //    });
    //    );           

    //private void TestAction1()
    //{
    //    Console.WriteLine("Test action 1");
    //}

    // Design goal pseudo code 2 - use it as a generic selector:
    //private void EnterTestMenu2() => new Menu<Component>()
    //    .Add("Comp 1", Component1)
    //    .Add("Comp 2", Component2)
    //    .Enter(result => 
    //    {
    //        Console.WriteLine($"Selected {result.Name}");
    //    });

    public abstract class MenuBase<T> : IDisposable 
        where T : MenuBase<T>
    {
        protected readonly List<MenuItem> _items = new();
        private int _selectedIndex = 0;
        private ConsoleColor _initialBackground;
        private ConsoleColor _initialForeground;
        private string _initialTitle = string.Empty;

        public ConsoleColor Background { get; set; } = ConsoleColor.Black;

        public ConsoleColor Foreground { get; set; } = ConsoleColor.Gray;

        public ConsoleColor? SelectedBackgroundColor { get; set; } = null;

        public ConsoleColor? SelectedForegroundColor { get; set; } = null;

        public char? SelectorChar { get; set; } = '>';

        public string Title { get; set; } = "ConsoleIO";

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
            SetSelectedBackgroundColor(options.SelectedBackcolor);
            SetSelectedForegroundColor(options.SelectedForecolor);
            SetSelectorChar(options.SelectorChar);
            Console.Clear();
        }

        private void StoreInitialSettings()
        {
            _initialBackground = Console.BackgroundColor;
            _initialForeground = Console.ForegroundColor;
            _initialTitle = Console.Title;
        }

        private void RestoreInitialSettings()
        {
            Console.BackgroundColor = _initialBackground;
            Console.ForegroundColor = _initialForeground;
            Console.Title = _initialTitle;
            Console.Clear();
        }

        public void Dispose() => RestoreInitialSettings();
    }
}