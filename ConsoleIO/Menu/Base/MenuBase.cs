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
        private readonly List<MenuItem> _items = new();
        private ConsoleColor _initialBackground;
        private ConsoleColor _initialForeground;
        private string _initialTitle = string.Empty;

        public ConsoleColor Background { get; private set; } = ConsoleColor.Black;

        public ConsoleColor Foreground { get; private set; } = ConsoleColor.Gray;

        public string Title { get; private set; } = "ConsoleIO";

        public MenuBase() => StoreInitialSettings();

        public MenuBase(Action<ConsoleMenuSettings> settings)
        {
            StoreInitialSettings();

            var menuSettings = new ConsoleMenuSettings();
            settings(menuSettings); // Invoke the setup delegate
            ApplySettings(menuSettings); // Apply the options to the console
        }

        ~MenuBase() => RestoreInitialSettings();

        protected void AddItem(MenuItem item) => _items.Add(item);

        public T Enter()
        {
            return (T)this;
        }

        public T AddMessage(string name, Action<MenuItemSettings>? settings = null) =>
            AddMessage(name, out _, settings);

        public T AddMessage(string name, out MenuMessage message, Action<MenuItemSettings>? settings = null)
        {
            message = new MenuMessage(name, settings);
            _items.Add(message);
            return (T)this;
        }

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
        }

        public void Dispose() => RestoreInitialSettings();
    }
}