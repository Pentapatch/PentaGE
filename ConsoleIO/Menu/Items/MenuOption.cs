namespace ConsoleIO
{
    public sealed class MenuOption : MenuItem
    {
        public MenuOption(ConsoleMenu owner, string text, Action<MenuOptionSettings>? settings, Action? action = null) : base(owner, text, action)
        {
            if (settings is null) return;

            var itemSettings = new MenuOptionSettings();
            settings(itemSettings);      // Invoke the setup delegate
            ApplySettings(itemSettings); // Apply the derived settings to the menu item
        }

        protected override void ApplySettings(MenuItemSettings settings)
        {
            // Apply base options
            base.ApplySettings(settings);

            // Apply derrived settings
            if (settings is MenuOptionSettings derrivedSettings)
            {
                // TODO: Apply derrived settings or remove this
                // code block if there is none to apply
            }
        }
    }
}