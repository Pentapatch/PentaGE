namespace ConsoleIO
{
    public sealed class ConsoleMenu : MenuBase<ConsoleMenu>
    {
        public MenuItem SelectedItem { get; private set; } = null!;

        internal ConsoleMenu(Action<ConsoleMenuSettings>? settings) : base(settings) { }

        public static ConsoleMenu Create(out ConsoleMenu menu, Action<ConsoleMenuSettings>? settings = null)
        {
            menu = new ConsoleMenu(settings);
            return menu;
        }

        public static ConsoleMenu Create(Action<ConsoleMenuSettings>? settings = null) =>
            new(settings);

        public ConsoleMenu Enter(Action? action = null)
        {
            // Early exit if there are no items
            if (_items.Count == 0) return this;

            TrySelectFirstAvailableItem();

            while (true)
            {
                if (NavigateMenu()) break;
            }

            // Invoke the final action before exiting
            action?.Invoke();

            return this;
        }

        private bool NavigateMenu()
        {
            while (true)
            {
                Console.Clear();
                WriteItems();
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        TryAdvanceSelection(upwards: true);
                        return false;
                    case ConsoleKey.DownArrow:
                        TryAdvanceSelection(upwards: false);
                        return false;
                    case ConsoleKey.Enter:
                        if (TriggerItem())
                            return true;
                        else
                            break;
                }
            }
        }

        private bool TriggerItem()
        {
            if (SelectedItem is MenuCheck check)
            {
                check.Checked = !check.Checked;
                SelectedItem.Action?.Invoke();
                return false;
            }

            SelectedItem.Action?.Invoke();

            return true;
        }

        private void TryAdvanceSelection(bool upwards)
        {
            if (upwards)
                TrySelectPreviousItem();
            else
                TrySelectNextItem();
        }

        private void TrySelectNextItem()
        {
            if (SelectedItem is null) return;

            for (int i = _items.IndexOf(SelectedItem) + 1; i < _items.Count; i++)
            {
                var item = _items[i];

                if (item is MenuMessage) continue;
                if (item.Enabled)
                {
                    item.Select();
                    return;
                }
            }

            if (!WrapArround) return;

            for (int i = 0; i < _items.IndexOf(SelectedItem); i++)
            {
                var item = _items[i];

                if (item is MenuMessage) continue;
                if (item.Enabled)
                {
                    item.Select();
                    return;
                }
            }
        }

        private void TrySelectPreviousItem()
        {
            if (SelectedItem is null) return;

            for (int i = _items.IndexOf(SelectedItem) - 1; i >= 0; i--)
            {
                var item = _items[i];

                if (item is MenuMessage) continue;
                if (item.Enabled)
                {
                    item.Select();
                    return;
                }
            }

            if (!WrapArround) return;

            for (int i = _items.Count - 1; i > _items.IndexOf(SelectedItem); i++)
            {
                var item = _items[i];

                if (item is MenuMessage) continue;
                if (item.Enabled)
                {
                    item.Select();
                    return;
                }
            }
        }

        private void TrySelectFirstAvailableItem()
        {
            if (SelectedItem is not null) return;

            foreach (var item in _items)
            {
                if (item is MenuMessage) continue;
                if (item.Enabled)
                {
                    item.Select();
                    break;
                }
            }
        }

        private void WriteItems() =>
            _items.ForEach(WriteItem);

        private void WriteItem(MenuItem item)
        {
            // Set up colors
            Console.ForegroundColor = item.Selected
                ? SelectedForegroundColor ?? item.Foreground ?? Foreground
                : !item.Enabled
                ? DisabledForegroundColor ?? item.Foreground ?? Foreground
                : item.Foreground ?? Foreground;
            Console.BackgroundColor = item.Selected
                ? SelectedBackgroundColor ?? item.Background ?? Background
                : !item.Enabled
                ? DisabledBackgroundColor ?? item.Background ?? Background
                : item.Background ?? Background;

            // Set up message colors
            // TODO: Refactor this so that the block above is not executed
            if (item is MenuMessage && item.Foreground is null && MessageForeground is not null)
                Console.ForegroundColor = MessageForeground.Value;
            if (item is MenuMessage && item.Background is null && MessageBackground is not null)
                Console.BackgroundColor = MessageBackground.Value;

            // Set up text
            string text = SelectorChar is char selectorChar && item.Selected
                ? $"{selectorChar} {item.Text}"
                : item.Text;

            if (item is MenuCheck check)
            {
                text = $"[{(check.Checked ? "X" : " ")}] {text}";
            }

            Console.WriteLine(text);
        }

        // NOTE: All overloads of the .Add...() methods must
        // follow the same pattern:
        // 1) name, (settings)
        // 2) name, action
        // 3) name, settings, action
        // 4) name, out var, (settings)
        // 5) name, out var, action
        // 6) name, out var, settings, action

        #region Add checkbox

        // name, (settings)
        public ConsoleMenu AddCheckbox(string name, Action<MenuCheckSettings>? settings = null) =>
            AddCheckbox(name, checkbox: out _, settings, action: null);

        // name, action
        public ConsoleMenu AddCheckbox(string name, Action action) =>
            AddCheckbox(name, checkbox: out _, action);

        // name, settings, action
        public ConsoleMenu AddCheckbox(string name, Action<MenuCheckSettings> settings, Action action) =>
            AddCheckbox(name, checkbox: out _, settings, action: action);

        // name, out var, (settings)
        public ConsoleMenu AddCheckbox(string name, out MenuCheck checkbox, Action<MenuCheckSettings>? settings = null) =>
            AddCheckbox(name, out checkbox, settings, action: null);

        // name, out var, action
        public ConsoleMenu AddCheckbox(string name, out MenuCheck checkbox, Action action) =>
            AddCheckbox(name, out checkbox, settings: null, action);

        // name, out var, settings, action
        public ConsoleMenu AddCheckbox(string name, out MenuCheck checkbox, Action<MenuCheckSettings>? settings = null, Action? action = null)
        {
            checkbox = new MenuCheck(this, name, settings, action);
            AddItem(checkbox);
            return this;
        }

        #endregion

        #region Add option

        // name, (settings)
        public ConsoleMenu AddOption(string text, Action<MenuOptionSettings>? settings = null) =>
            AddOption(text, option: out _, settings, action: null);

        // name, action
        public ConsoleMenu AddOption(string text, Action action) =>
            AddOption(text, option: out _, settings: null, action);

        // name, settings, action
        public ConsoleMenu AddOption(string text, Action<MenuOptionSettings> settings, Action action) =>
            AddOption(text, option: out _, settings, action: action);

        // name, out var, (settings)
        public ConsoleMenu AddOption(string text, out MenuOption option, Action<MenuOptionSettings>? settings = null) =>
            AddOption(text, out option, settings, action: null);

        // name, out var, action
        public ConsoleMenu AddOption(string text, out MenuOption option, Action action) =>
            AddOption(text, out option, settings: null, action);

        // name, out var, settings, action
        public ConsoleMenu AddOption(string text, out MenuOption option, Action<MenuOptionSettings>? settings = null, Action? action = null)
        {
            option = new MenuOption(this, text, settings, action);
            AddItem(option);
            return this;
        }

        #endregion

        internal void Select(MenuItem item)
        {
            if (!item.Enabled) return;

            if (SelectedItem is not null) SelectedItem.Selected = false;
            item.Selected = true;
            SelectedItem = item;
        }

        internal void Remove(MenuItem item)
        {
            if (item.Selected)
            {
                SelectedItem = null!;
            }

            _items.Remove(item);
        }
    }
}