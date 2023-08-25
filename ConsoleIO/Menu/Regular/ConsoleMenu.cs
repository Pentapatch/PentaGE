using System;

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

            // Select the first item if none are selected
            SelectFirstItem();

            foreach (var item in _items)
            {
                WriteItem(item);
            }

            Console.ReadKey(true);

            // Invoke the final action before exiting
            action?.Invoke();

            return this;
        }

        private void SelectFirstItem()
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

        private void WriteItem(MenuItem item)
        {
            Console.WriteLine(item.Text);
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