namespace ConsoleIO
{
    public sealed class ConsoleMenu : MenuBase<ConsoleMenu>
    {
        public ConsoleMenu() { }

        public ConsoleMenu(Action<ConsoleMenuSettings> settings) : base(settings) { }

        public ConsoleMenu AddCheckbox(string name, Action<MenuCheckSettings>? settings = null) => 
            AddCheckbox(name, out _, settings);

        public ConsoleMenu AddCheckbox(string name, out MenuCheck checkbox, Action<MenuCheckSettings>? settings = null)
        {
            checkbox = new MenuCheck(name, settings);
            AddItem(checkbox);
            return this;
        }

        public ConsoleMenu AddOption(string text, Action<MenuOptionSettings>? settings = null) =>
            AddOption(text, out _, settings);

        public ConsoleMenu AddOption(string text, out MenuOption option, Action<MenuOptionSettings>? settings = null)
        {
            option = new MenuOption(text, settings);
            AddItem(option);
            return this;
        }
    }
}